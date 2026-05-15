"""
=============================================================
  DO AN IOT NHA THONG MINH - SERIAL READER (COMPIM)
  
  Chuc nang:
    - Ket noi COM2 (COMPIM <-> Arduino Serial1)
    - Doc du lieu gui len tu Arduino (moi 1 giay)
    - Phan tich va luu vao MySQL qua DBHelper
    - Hien thi log ra console
    
  Cai dat thu vien:
    pip install pyserial mysql-connector-python bcrypt
    
  Chay:
    python serial_reader.py
    python serial_reader.py --port COM3 --baud 9600   # tuy chinh cong
=============================================================
"""

import serial
import serial.tools.list_ports
import time
import argparse
import sys
import threading
from datetime import datetime

# Import DBHelper tu cung thu muc
try:
    from db_helper import DBHelper
except ImportError:
    print("[LOI] Khong tim thay db_helper.py. Chay tu thu muc database/")
    sys.exit(1)


# ============================================================
# CAU HINH MAC DINH
# ============================================================
DEFAULT_PORT  = "COM2"
DEFAULT_BAUD  = 9600
DEFAULT_TIMEOUT = 2      # giay cho moi lan doc


# ============================================================
# PARSER LENH
# ============================================================
def parse_args():
    parser = argparse.ArgumentParser(
        description="Doc du lieu COMPIM tu Arduino va luu vao MySQL"
    )
    parser.add_argument(
        "--port", type=str, default=DEFAULT_PORT,
        help=f"Cong COM (mac dinh: {DEFAULT_PORT})"
    )
    parser.add_argument(
        "--baud", type=int, default=DEFAULT_BAUD,
        help=f"Baudrate (mac dinh: {DEFAULT_BAUD})"
    )
    parser.add_argument(
        "--no-db", action="store_true",
        help="Chi hien thi, khong luu vao database"
    )
    parser.add_argument(
        "--list-ports", action="store_true",
        help="Hien thi danh sach cong COM kha dung roi thoat"
    )
    return parser.parse_args()


# ============================================================
# HIEN THI DANH SACH COM
# ============================================================
def list_available_ports():
    ports = serial.tools.list_ports.comports()
    if not ports:
        print("Khong tim thay cong COM nao.")
        return
    print("\n=== DANH SACH CONG COM KHA DUNG ===")
    for p in ports:
        print(f"  {p.device:10s}  {p.description}")
    print("====================================\n")


# ID ban ghi mo cua gan nhat (de cap nhat closed_at khi dong)
_last_door_access_id: int = None


# ============================================================
# XU LY TUNG DONG
# ============================================================
def process_line(line: str, db: DBHelper = None, save_to_db: bool = True):
    """
    Phan tich 1 dong nhan tu Arduino va luu vao DB.
    Moi nhanh xu ly deu duoc boc trong try/except rieng
    de mot loi DB khong lam crash vong lap chinh.
    """
    line = line.strip()
    if not line:
        return

    ts = datetime.now().strftime("%H:%M:%S.%f")[:-3]

    # ------- Hien thi raw -------
    print(f"[{ts}] << {line}")

    if not save_to_db or db is None:
        return

    # ------- Ban tin du lieu cam bien -------
    if line.startswith("KHI=") or line.startswith("STAT|"):
        try:
            ok = db.process_arduino_line(line)
            if ok:
                data = DBHelper.parse_arduino_line(line)
                print(f"         [DB] Sensor saved | Gas={data.get('KHI')} "
                      f"Temp={data.get('NHIET_DO')}C "
                      f"Level={data.get('MUC_CANH_BAO','?')}")
        except Exception as e:
            print(f"         [LOI-DB] Luu sensor that bai: {e}")
        return

    # ------- Canh bao an ninh (sai mat khau 3 lan) -------
    if "CANH_BAO=AN_NINH" in line:
        try:
            data = DBHelper.parse_arduino_line(line)
            alert_id = db.insert_alert(
                alert_type="SECURITY",
                level="NGUY_HIEM",
                message=data.get("NOI_DUNG", "SAI_MAT_KHAU_3_LAN")
            )
            print(f"         [DB] Alert SECURITY saved (id={alert_id})")
        except Exception as e:
            print(f"         [LOI-DB] Luu canh bao AN_NINH that bai: {e}")
        return

    # ------- Ket qua truy cap cua -------
    if "TRUY_CAP=" in line:
        global _last_door_access_id
        try:
            data = DBHelper.parse_arduino_line(line)
            result_raw = data.get("TRUY_CAP", "")
            cua_raw    = data.get("CUA", "")
            so_lan_raw = data.get("SO_LAN", "0")

            print(f"         [DEBUG] TRUY_CAP='{result_raw}' CUA='{cua_raw}' SO_LAN='{so_lan_raw}'")

            # --- Mo cua thanh cong ---
            if result_raw == "THANH_CONG" and cua_raw == "DANG_MO":
                access_id = db.insert_door_access(
                    source="KEYPAD",
                    result="SUCCESS",
                    role_matched="admin",
                    note="Mo cua thanh cong tu Arduino"
                )
                _last_door_access_id = access_id
                now_str = datetime.now().strftime('%H:%M:%S.%f')[:-3]
                print(f"         [DB] DoorAccess SUCCESS saved (id={access_id})")
                print(f"              opened_at = {now_str}")

            # --- Dong cua ---
            elif result_raw == "THANH_CONG" and cua_raw == "DA_KHOA":
                if _last_door_access_id:
                    db.update_door_closed(_last_door_access_id)
                    now_str = datetime.now().strftime('%H:%M:%S.%f')[:-3]
                    print(f"         [DB] DoorAccess CLOSED (id={_last_door_access_id})")
                    print(f"              closed_at = {now_str} | duration tu dong tinh boi MySQL")
                    _last_door_access_id = None

            # --- Sai mat khau ---
            elif result_raw == "TU_CHOI":
                try:
                    wrong = int(so_lan_raw)
                except ValueError:
                    wrong = 0
                locked = wrong >= 3
                status = "LOCKED" if locked else "FAILED"
                db.insert_door_access(
                    source="KEYPAD",
                    result=status,
                    role_matched="none",
                    wrong_count=wrong,
                    note=f"Sai mat khau lan {wrong}"
                )
                print(f"         [DB] DoorAccess {status} (lan {wrong}) saved")

            else:
                print(f"         [WARN] Khong nhan dang duoc TRUY_CAP: '{line}'")

        except Exception as e:
            print(f"         [LOI-DB] Xu ly TRUY_CAP that bai: {e}")
        return

    # ------- Ban tin lenh (LENH=...) -------
    if line.startswith("LENH="):
        try:
            data = DBHelper.parse_arduino_line(line)
            cmd = data.get("LENH", "UNKNOWN")
            ket_qua = data.get("KET_QUA", "")
            cmd_id = db.insert_command(
                source="SYSTEM",
                command=cmd,
                parameters=line
            )
            db.mark_command_result(cmd_id, ket_qua == "OK", response=line)
            print(f"         [DB] Command '{cmd}' saved")
        except Exception as e:
            print(f"         [LOI-DB] Luu lenh that bai: {e}")
        return

    # ------- Cac dong khac: chi in, khong luu -------
    # (KEYPAD=..., HE_THONG=..., LOI=..., v.v.)


# ============================================================
# VONG LAP DOC SERIAL CHINH
# ============================================================
def run_reader(port: str, baud: int, save_to_db: bool):
    print("=" * 60)
    print("  NHA THONG MINH - SERIAL READER")
    print(f"  Cong  : {port}")
    print(f"  Baud  : {baud}")
    print(f"  Luu DB: {'CO' if save_to_db else 'KHONG (--no-db)'}")
    print("=" * 60)

    # --- Khoi tao DB ---
    db = None
    if save_to_db:
        try:
            db = DBHelper()
            # Thu ket noi
            db.get_device_status()
            print("[DB ] Ket noi MySQL thanh cong.")
        except Exception as e:
            print(f"[LOI] Khong ket noi duoc MySQL: {e}")
            print("      Tiep tuc che do hien thi (khong luu DB).")
            db = None
            save_to_db = False

    # --- Mo cong COM ---
    ser = None
    while True:
        try:
            ser = serial.Serial(
                port=port,
                baudrate=baud,
                timeout=DEFAULT_TIMEOUT
            )
            print(f"[COM] Da ket noi {port} @ {baud} baud")
            print("      Nhan Ctrl+C de thoat.\n")
            break
        except serial.SerialException as e:
            print(f"[LOI] Khong mo duoc {port}: {e}")
            print("      Thu lai sau 5 giay... (Ctrl+C de thoat)")
            try:
                time.sleep(5)
            except KeyboardInterrupt:
                print("\n[THOAT] Nguoi dung nhan Ctrl+C.")
                sys.exit(0)


    def try_send_pending_commands():
        """
        Web (và WinForms) chỉ ghi vào DB bảng control_commands.
        Reader này sẽ:
          - Lấy command PENDING (theo thứ tự tạo)
          - Gửi xuống COM đúng command string (FAN_ON, DOOR_OPEN, ...)
          - Cập nhật SENT / SUCCESS / FAILED
        """
        if not save_to_db or db is None:
            return

        # Tránh trùng luồng gửi lệnh: Flask/webapp đã bridge PENDING theo source='MOBILE'
        pending = db.get_pending_commands(limit=10, exclude_source="MOBILE")
        if not pending:
            return

        for cmd_row in pending:
            command_id = cmd_row["id"]
            command = cmd_row.get("command")
            status_before = cmd_row.get("status")

            if not command:
                db.mark_command_result(command_id, success=False, response="EMPTY_COMMAND")
                continue

            # Arduino hiện tại KHÔNG gửi ACK kiểu LENH=/KET_QUA=...
            # => coi như thành công nếu TX gửi được lên serial.
            try:
                ser.write((str(command) + "\n").encode("ascii", errors="replace"))
                db.mark_command_sent(command_id)
                db.mark_command_result(command_id, success=True, response="TX_OK")
                print(f"         [TX-DB] Sent command id={command_id} cmd={command} (status was {status_before}) -> SUCCESS")
            except Exception as txe:
                db.mark_command_result(command_id, success=False, response=f"TX_ERROR: {txe}")
                print(f"         [TX-LOI] command id={command_id} cmd={command} err={txe}")
                continue

    # --- Vong lap doc ---
    try:
        buf = ""
        while True:
            try:
                # 1) Ưu tiên xử lý RX nếu có dữ liệu
                if ser.in_waiting > 0:
                    raw = ser.read(ser.in_waiting)
                    text = raw.decode("utf-8", errors="replace")
                    buf += text

                    # Xu ly tung dong ket thuc bang \n
                    while "\n" in buf:
                        line, buf = buf.split("\n", 1)
                        line = line.replace("\r", "")
                        try:
                            process_line(line, db=db, save_to_db=save_to_db)
                        except Exception as pe:
                            print(f"[LOI] process_line crash: {pe}")
                else:
                    # 2) Không có RX: thỉnh thoảng poll DB để gửi lệnh điều khiển
                    # (web/WinForms tạo PENDING, reader sẽ gửi xuống COM)
                    try_send_pending_commands()
                    time.sleep(0.05)

            except serial.SerialException as e:
                print(f"\n[LOI] Mat ket noi serial: {e}")
                print("      Thu ket noi lai sau 5 giay...")
                time.sleep(5)
                try:
                    ser.close()
                    ser.open()
                    print(f"[COM] Ket noi lai {port} thanh cong.")
                except Exception as re:
                    print(f"[LOI] Ket noi lai that bai: {re}")
            except Exception as e:
                print(f"[LOI] Loi bat ngo trong vong lap: {e}")

    except KeyboardInterrupt:
        print("\n\n[THOAT] Nguoi dung nhan Ctrl+C.")
    finally:
        if ser and ser.is_open:
            ser.close()
            print(f"[COM] Da dong {port}.")
        print("[XONG] Chuong trinh ket thuc.")


# ============================================================
# MAIN
# ============================================================
if __name__ == "__main__":
    args = parse_args()

    if args.list_ports:
        list_available_ports()
        sys.exit(0)

    run_reader(
        port=args.port,
        baud=args.baud,
        save_to_db=not args.no_db
    )
