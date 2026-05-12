"""
=============================================================
  DO AN IOT NHA THONG MINH - DATABASE HELPER (CRUD) v2
  
  Thay doi so phien ban cu:
    - add_account : bo tham so email
    - insert_door_access : bo ip_address, password_len
    - insert_command : bo enum 'API'
    - list_accounts : bo truong email khoi SELECT
    - Tat ca NOW() -> NOW(3) cho ms precision

  Su dung:
      from db_helper import DBHelper
      db = DBHelper()
      db.insert_sensor_data(gas=320, temp=36.5, pir=1, rain=0)
=============================================================
"""

import mysql.connector
from mysql.connector import Error
from datetime import datetime
import bcrypt
from typing import Optional, Dict, Any, List


# ============================================================
# CAU HINH KET NOI
# ============================================================
DB_CONFIG = {
    "host":     "localhost",
    "port":     3306,
    "user":     "root",
    "password": "123456789@",
    "database": "nha_thong_minh",
    "charset":  "utf8mb4",
}


class DBHelper:
    """
    Lop tien ich thao tac voi MySQL cho he thong Nha Thong Minh.
    Moi phuong thuc mo/dong ket noi rieng biet (khong giu ket noi thuong tru).
    """

    def __init__(self, config: dict = None):
        self.config = config or DB_CONFIG

    # ----------------------------------------------------------
    # INTERNAL
    # ----------------------------------------------------------
    def _connect(self):
        return mysql.connector.connect(**self.config)

    def _execute(self, sql: str, params: tuple = (), fetch: bool = False):
        conn = self._connect()
        try:
            cur = conn.cursor(dictionary=True)
            cur.execute(sql, params)
            if fetch:
                return cur.fetchall()
            else:
                conn.commit()
                return cur.lastrowid
        except Error as e:
            conn.rollback()
            raise e
        finally:
            conn.close()

    # ----------------------------------------------------------
    # 1. TAI KHOAN - ACCOUNTS
    # ----------------------------------------------------------
    def get_account(self, username: str) -> Optional[Dict]:
        """Lay thong tin tai khoan theo username."""
        rows = self._execute(
            "SELECT * FROM `accounts` WHERE `username` = %s AND `is_active` = 1",
            (username,), fetch=True
        )
        return rows[0] if rows else None

    def verify_login(self, username: str, plain_password: str) -> Optional[Dict]:
        """
        Kiem tra dang nhap.
        Tra ve dict tai khoan neu thanh cong, None neu that bai.
        """
        account = self.get_account(username)
        if not account:
            return None
        ok = bcrypt.checkpw(
            plain_password.encode("utf-8"),
            account["password_hash"].encode("utf-8")
        )
        if ok:
            self._execute(
                "UPDATE `accounts` SET `last_login` = NOW(3) WHERE `id` = %s",
                (account["id"],)
            )
            return account
        return None

    def add_account(self, username: str, plain_password: str,
                    role: str = "viewer", full_name: str = None) -> int:
        """Them tai khoan moi. Tra ve id cua ban ghi moi."""
        hashed = bcrypt.hashpw(
            plain_password.encode("utf-8"), bcrypt.gensalt()
        ).decode("utf-8")
        return self._execute(
            """INSERT INTO `accounts`
                   (`username`, `password_hash`, `role`, `full_name`)
               VALUES (%s, %s, %s, %s)""",
            (username, hashed, role, full_name)
        )

    def change_password(self, account_id: int, new_plain_password: str):
        """Doi mat khau."""
        hashed = bcrypt.hashpw(
            new_plain_password.encode("utf-8"), bcrypt.gensalt()
        ).decode("utf-8")
        self._execute(
            "UPDATE `accounts` SET `password_hash` = %s WHERE `id` = %s",
            (hashed, account_id)
        )

    def list_accounts(self) -> List[Dict]:
        """Lay danh sach tat ca tai khoan (khong tra ve password_hash)."""
        return self._execute(
            "SELECT `id`,`username`,`role`,`full_name`,"
            "`is_active`,`last_login`,`created_at` FROM `accounts` ORDER BY `id`",
            fetch=True
        )

    def deactivate_account(self, account_id: int):
        """Vo hieu hoa tai khoan (khong xoa)."""
        self._execute(
            "UPDATE `accounts` SET `is_active` = 0 WHERE `id` = %s",
            (account_id,)
        )

    # ----------------------------------------------------------
    # 2. DU LIEU CAM BIEN - SENSOR DATA
    # ----------------------------------------------------------
    def insert_sensor_data(self, gas: int, temp: float,
                           pir: int, rain: int, system_level: str = "AN_TOAN") -> int:
        """
        Luu du lieu cam bien vao bang sensor_data.
        recorded_at duoc MySQL tu dong dat = CURRENT_TIMESTAMP(3).
        """
        return self._execute(
            """INSERT INTO `sensor_data`
                   (`gas_value`, `temperature`, `pir_status`, `rain_status`, `system_level`)
               VALUES (%s, %s, %s, %s, %s)""",
            (gas, temp, pir, rain, system_level)
        )

    def get_sensor_data(self, limit: int = 100,
                        from_dt: datetime = None,
                        to_dt: datetime = None) -> List[Dict]:
        """Lay du lieu cam bien, co the loc theo khoang thoi gian."""
        sql = "SELECT * FROM `sensor_data`"
        params = []
        conditions = []

        if from_dt:
            conditions.append("`recorded_at` >= %s")
            params.append(from_dt)
        if to_dt:
            conditions.append("`recorded_at` <= %s")
            params.append(to_dt)
        if conditions:
            sql += " WHERE " + " AND ".join(conditions)

        sql += " ORDER BY `recorded_at` DESC LIMIT %s"
        params.append(limit)
        return self._execute(sql, tuple(params), fetch=True)

    def get_latest_sensor(self) -> Optional[Dict]:
        """Lay du lieu cam bien moi nhat."""
        rows = self._execute(
            "SELECT * FROM `sensor_data` ORDER BY `recorded_at` DESC LIMIT 1",
            fetch=True
        )
        return rows[0] if rows else None

    # ----------------------------------------------------------
    # 3. TRANG THAI THIET BI - DEVICE STATUS (row id=1)
    # ----------------------------------------------------------
    def get_device_status(self) -> Optional[Dict]:
        """Lay trang thai hien tai cua toan bo thiet bi."""
        rows = self._execute(
            "SELECT * FROM `device_status` WHERE `id` = 1",
            fetch=True
        )
        return rows[0] if rows else None

    def update_device_status(self, **kwargs):
        """
        Cap nhat trang thai thiet bi.
        Vi du: update_device_status(fan_status=1, door_status=0)
        updated_at duoc MySQL tu dong cap nhat = CURRENT_TIMESTAMP(3).
        """
        if not kwargs:
            return
        allowed = {
            "fan_status", "door_status", "window_status",
            "buzzer_status", "led_state", "auto_mode",
            "system_level", "last_message"
        }
        fields = {k: v for k, v in kwargs.items() if k in allowed}
        if not fields:
            return
        set_clause = ", ".join(f"`{k}` = %s" for k in fields)
        self._execute(
            f"UPDATE `device_status` SET {set_clause} WHERE `id` = 1",
            tuple(fields.values())
        )

    def sync_device_status_from_arduino(self, data: Dict):
        """Cap nhat trang thai tu ban tin Arduino da parse."""
        fan    = 1 if data.get("QUAT")       == "BAT"     else 0
        door   = 1 if data.get("CUA_CHINH")  == "MO"      else 0
        window = 1 if data.get("CUA_SO")     == "MO"      else 0
        buzzer = 1 if data.get("COI_BAO")    == "BAT"     else 0
        auto   = 1 if data.get("CHE_DO")     == "TU_DONG" else 0
        level_map = {
            "AN TOAN":  "AN_TOAN",
            "CANH BAO": "CANH_BAO",
            "NGUY HIEM":"NGUY_HIEM",
        }
        level = level_map.get(data.get("MUC_CANH_BAO", "AN TOAN"), "AN_TOAN")
        led_map = {"AN_TOAN": "SAFE", "CANH_BAO": "WARNING", "NGUY_HIEM": "DANGER"}

        self.update_device_status(
            fan_status=fan, door_status=door, window_status=window,
            buzzer_status=buzzer, auto_mode=auto,
            system_level=level, led_state=led_map.get(level, "SAFE"),
            last_message=data.get("THONG_BAO", "")
        )

    # ----------------------------------------------------------
    # 4. LICH SU CANH BAO - ALERT HISTORY
    # ----------------------------------------------------------
    def insert_alert(self, alert_type: str, level: str, message: str,
                     gas: int = None, temp: float = None,
                     pir: int = None, rain: int = None) -> int:
        """
        Them ban ghi canh bao.
        created_at duoc MySQL tu dong dat = CURRENT_TIMESTAMP(3).
        """
        return self._execute(
            """INSERT INTO `alert_history`
                   (`alert_type`,`level`,`message`,`gas_value`,
                    `temperature`,`pir_status`,`rain_status`)
               VALUES (%s, %s, %s, %s, %s, %s, %s)""",
            (alert_type, level, message, gas, temp, pir, rain)
        )

    def resolve_alert(self, alert_id: int):
        """Danh dau canh bao da duoc xu ly (luu thoi diem ms)."""
        self._execute(
            "UPDATE `alert_history` SET `resolved_at` = NOW(3) WHERE `id` = %s",
            (alert_id,)
        )

    def get_active_alerts(self) -> List[Dict]:
        """Lay danh sach canh bao chua duoc xu ly."""
        return self._execute(
            "SELECT * FROM `alert_history` WHERE `resolved_at` IS NULL"
            " ORDER BY `created_at` DESC",
            fetch=True
        )

    def get_alert_history(self, limit: int = 50) -> List[Dict]:
        """Lay lich su canh bao (moi nhat truoc)."""
        return self._execute(
            "SELECT * FROM `alert_history` ORDER BY `created_at` DESC LIMIT %s",
            (limit,), fetch=True
        )

    # ----------------------------------------------------------
    # 5. LENH DIEU KHIEN - CONTROL COMMANDS
    # ----------------------------------------------------------
    def insert_command(self, source: str, command: str,
                       account_id: int = None,
                       parameters: str = None) -> int:
        """
        Ghi nhan lenh dieu khien moi.
        source: 'WINFORM' | 'MOBILE' | 'TERMITE' | 'SYSTEM'
        created_at duoc MySQL tu dong dat = CURRENT_TIMESTAMP(3).
        """
        return self._execute(
            """INSERT INTO `control_commands`
                   (`source`,`account_id`,`command`,`parameters`)
               VALUES (%s, %s, %s, %s)""",
            (source, account_id, command, parameters)
        )

    def mark_command_sent(self, command_id: int):
        """Cap nhat trang thai lenh sang SENT va luu sent_at (ms)."""
        self._execute(
            "UPDATE `control_commands` SET `status`='SENT', `sent_at`=NOW(3)"
            " WHERE `id` = %s",
            (command_id,)
        )

    def mark_command_result(self, command_id: int,
                            success: bool, response: str = None):
        """Cap nhat ket qua lenh (SUCCESS/FAILED) va responded_at (ms)."""
        status = "SUCCESS" if success else "FAILED"
        self._execute(
            "UPDATE `control_commands`"
            " SET `status`=%s, `response`=%s, `responded_at`=NOW(3)"
            " WHERE `id` = %s",
            (status, response, command_id)
        )

    def get_recent_commands(self, limit: int = 50) -> List[Dict]:
        """Lay lich su lenh dieu khien gan nhat kem ten nguoi gui."""
        return self._execute(
            """SELECT c.*, a.`username`, a.`role`
               FROM `control_commands` c
               LEFT JOIN `accounts` a ON c.`account_id` = a.`id`
               ORDER BY c.`created_at` DESC LIMIT %s""",
            (limit,), fetch=True
        )

    # ----------------------------------------------------------
    # 6. LICH SU MO CUA - DOOR ACCESS LOG
    # ----------------------------------------------------------
    def insert_door_access(self, source: str, result: str,
                           role_matched: str = "none",
                           wrong_count: int = 0,
                           note: str = None) -> int:
        """
        Ghi nhan mot lan truy cap cua chinh.
        opened_at duoc MySQL tu dong dat = CURRENT_TIMESTAMP(3).
        Tra ve id de dung cho update_door_closed() sau do.
        """
        return self._execute(
            """INSERT INTO `door_access_log`
                   (`input_source`,`role_matched`,`result`,`wrong_count`,`note`)
               VALUES (%s, %s, %s, %s, %s)""",
            (source, role_matched, result, wrong_count, note)
        )

    def update_door_closed(self, access_id: int):
        """
        Cap nhat thoi diem dong cua (ms precision).
        duration_sec se duoc MySQL tu dong tinh.
        """
        self._execute(
            "UPDATE `door_access_log` SET `closed_at` = NOW(3) WHERE `id` = %s",
            (access_id,)
        )

    def get_door_access_log(self, limit: int = 100,
                            result_filter: str = None) -> List[Dict]:
        """Lay lich su mo cua, co the loc theo ket qua."""
        if result_filter:
            return self._execute(
                "SELECT * FROM `door_access_log` WHERE `result` = %s"
                " ORDER BY `opened_at` DESC LIMIT %s",
                (result_filter, limit), fetch=True
            )
        return self._execute(
            "SELECT * FROM `door_access_log` ORDER BY `opened_at` DESC LIMIT %s",
            (limit,), fetch=True
        )

    # ----------------------------------------------------------
    # 7. CAU HINH NGUONG - THRESHOLD CONFIG
    # ----------------------------------------------------------
    def get_threshold(self, key: str, default=None):
        """Lay gia tri nguong theo key, tu dong ep kieu."""
        rows = self._execute(
            "SELECT `config_value`, `data_type` FROM `threshold_config`"
            " WHERE `config_key` = %s",
            (key,), fetch=True
        )
        if not rows:
            return default
        val, dtype = rows[0]["config_value"], rows[0]["data_type"]
        try:
            if dtype == "INT":   return int(val)
            if dtype == "FLOAT": return float(val)
            if dtype == "BOOL":  return bool(int(val))
            return val
        except (ValueError, TypeError):
            return default

    def get_all_thresholds(self) -> Dict[str, Any]:
        """Lay tat ca nguong duoi dang dict {key: value}."""
        rows = self._execute(
            "SELECT `config_key`, `config_value`, `data_type`"
            " FROM `threshold_config` ORDER BY `config_key`",
            fetch=True
        )
        result = {}
        for row in rows:
            val, dtype = row["config_value"], row["data_type"]
            try:
                if dtype == "INT":   val = int(val)
                elif dtype == "FLOAT": val = float(val)
                elif dtype == "BOOL":  val = bool(int(val))
            except (ValueError, TypeError):
                pass
            result[row["config_key"]] = val
        return result

    def set_threshold(self, key: str, value, account_id: int = None):
        """Upsert gia tri nguong. updated_at tu dong cap nhat."""
        self._execute(
            """INSERT INTO `threshold_config` (`config_key`, `config_value`, `updated_by`)
               VALUES (%s, %s, %s)
               ON DUPLICATE KEY UPDATE
                   `config_value` = VALUES(`config_value`),
                   `updated_by`   = VALUES(`updated_by`)""",
            (key, str(value), account_id)
        )

    # ----------------------------------------------------------
    # TIEN ICH: PARSE CHUOI TU ARDUINO
    # ----------------------------------------------------------
    @staticmethod
    def parse_arduino_line(line: str) -> Dict[str, str]:
        """
        Chuyen chuoi Arduino sang dict.
        "KHI=320;NHIET_DO=36.5;..." -> {"KHI": "320", "NHIET_DO": "36.5", ...}
        """
        result = {}
        for part in line.strip().split(";"):
            if "=" in part:
                k, _, v = part.partition("=")
                result[k.strip()] = v.strip()
        return result

    def process_arduino_line(self, line: str, auto_alert: bool = True) -> bool:
        """
        Xu ly toan bo mot dong du lieu tu Arduino:
          - Parse chuoi
          - Luu sensor_data (recorded_at = NOW(3))
          - Cap nhat device_status (updated_at = NOW(3))
          - Tu dong tao alert neu can (created_at = NOW(3))
        Tra ve True neu xu ly thanh cong.
        """
        data = self.parse_arduino_line(line)
        if "KHI" not in data or "NHIET_DO" not in data:
            return False

        try:
            gas  = int(data["KHI"])
            temp = float(data["NHIET_DO"])
            pir  = int(data.get("CO_NGUOI", 0))
            rain = int(data.get("MUA", 0))
        except (ValueError, KeyError):
            return False

        level_map = {
            "AN TOAN":  "AN_TOAN",
            "CANH BAO": "CANH_BAO",
            "NGUY HIEM":"NGUY_HIEM",
        }
        level = level_map.get(data.get("MUC_CANH_BAO", "AN TOAN"), "AN_TOAN")

        self.insert_sensor_data(gas, temp, pir, rain, level)
        self.sync_device_status_from_arduino(data)

        if auto_alert and level in ("CANH_BAO", "NGUY_HIEM"):
            msg = data.get("THONG_BAO", level)
            alert_type = "GAS" if gas > 300 else "TEMPERATURE" if temp > 35 else "COMBO"
            self.insert_alert(alert_type, level, msg, gas, temp, pir, rain)

        return True


# ============================================================
# VI DU SU DUNG
# ============================================================
if __name__ == "__main__":
    db = DBHelper()

    print("\n--- Trang thai thiet bi ---")
    status = db.get_device_status()
    if status:
        print(f"  Level={status['system_level']} | Fan={status['fan_status']}"
              f" | Door={status['door_status']} | updated_at={status['updated_at']}")

    print("\n--- Tat ca nguong canh bao ---")
    for k, v in db.get_all_thresholds().items():
        print(f"  {k:20s} = {v}")

    print("\n--- Kiem tra dang nhap ---")
    acc = db.verify_login("admin", "1234")
    print("Login admin/1234:", "THANH CONG" if acc else "THAT BAI")

    print("\n--- Gia lap xu ly ban tin Arduino ---")
    sample = ("KHI=320;NHIET_DO=36.5;CO_NGUOI=1;MUA=0;"
              "QUAT=BAT;CUA_SO=DONG;CUA_CHINH=DONG;COI_BAO=TAT;"
              "CHE_DO=TU_DONG;MUC_CANH_BAO=CANH BAO;THONG_BAO=CANH BAO KHI")
    ok = db.process_arduino_line(sample)
    print("Xu ly ban tin:", "OK" if ok else "THAT BAI")

    print("\n--- Cam bien moi nhat ---")
    latest = db.get_latest_sensor()
    if latest:
        print(f"  Gas={latest['gas_value']}, Temp={latest['temperature']}C,"
              f" recorded_at={latest['recorded_at']}")

    print("\n--- Canh bao chua xu ly ---")
    alerts = db.get_active_alerts()
    print(f"  Co {len(alerts)} canh bao chua xu ly.")
