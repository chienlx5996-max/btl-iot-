"""
=============================================================
  DO AN IOT NHA THONG MINH - DATABASE SETUP (TOI UU HOA)
  MySQL + Python (mysql-connector-python)

  Thay doi so voi phien ban cu:
    - accounts     : Bo truong `email` (khong dung trong UI)
    - control_commands : Bo enum 'API' khoi source
    - door_access_log  : Bo `ip_address` va `password_len`
    - Tat ca timestamp su dung DATETIME(3) (ms precision)

  Cai dat thu vien:
      pip install mysql-connector-python bcrypt

  Chay lan dau de tao CSDL va cac bang:
      python db_setup.py
=============================================================
"""

import mysql.connector
from mysql.connector import Error
import bcrypt
import sys
from datetime import datetime, timedelta, timezone

# ============================================================
# CAU HINH KET NOI
# ============================================================
DB_CONFIG = {
    "host": "localhost",
    "port": 3306,
    "user": "root",
    "password": "123456789@",
    "charset": "utf8mb4",
}

DB_NAME = "nha_thong_minh"


# ============================================================
# SQL TAO BANG (TOI UU HOA)
# ============================================================

SQL_CREATE_DB = (
    f"CREATE DATABASE IF NOT EXISTS `{DB_NAME}` "
    "CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
)

# -----------------------------------------------------------
# 1. TAI KHOAN - bo truong email (khong dung trong UI)
# -----------------------------------------------------------
SQL_ACCOUNTS = """
CREATE TABLE IF NOT EXISTS `accounts` (
    `id`            INT AUTO_INCREMENT PRIMARY KEY,
    `username`      VARCHAR(50)  NOT NULL UNIQUE,
    `password_hash` VARCHAR(255) NOT NULL,
    `role`          ENUM('admin','user','viewer') NOT NULL DEFAULT 'viewer'
                    COMMENT 'admin: toan quyen | user: dieu khien | viewer: chi xem',
    `full_name`     VARCHAR(100) DEFAULT NULL,
    `is_active`     TINYINT(1)   NOT NULL DEFAULT 1,
    `last_login`    DATETIME     DEFAULT NULL
                    COMMENT 'Thoi diem dang nhap cuoi cung',
    `created_at`    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at`    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP
                    ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB COMMENT='Tai khoan nguoi dung va phan quyen';
"""

# -----------------------------------------------------------
# 2. DU LIEU CAM BIEN
# -----------------------------------------------------------
SQL_SENSOR_DATA = """
CREATE TABLE IF NOT EXISTS `sensor_data` (
    `id`            BIGINT AUTO_INCREMENT PRIMARY KEY,
    `gas_value`     INT          NOT NULL COMMENT 'Gia tri MQ-3 (ADC 0-1023)',
    `temperature`   FLOAT        NOT NULL COMMENT 'Nhiet do LM35 (do C)',
    `pir_status`    TINYINT(1)   NOT NULL DEFAULT 0 COMMENT '1=Co nguoi',
    `rain_status`   TINYINT(1)   NOT NULL DEFAULT 0 COMMENT '1=Co mua',
    `system_level`  ENUM('AN_TOAN','CANH_BAO','NGUY_HIEM') NOT NULL DEFAULT 'AN_TOAN',
    `recorded_at`   DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
                    COMMENT 'Thoi diem do (ms precision)',
    INDEX `idx_recorded_at` (`recorded_at`),
    INDEX `idx_system_level` (`system_level`)
) ENGINE=InnoDB COMMENT='Du lieu cam bien theo thoi gian';
"""

# -----------------------------------------------------------
# 3. TRANG THAI THIET BI
# -----------------------------------------------------------
SQL_DEVICE_STATUS = """
CREATE TABLE IF NOT EXISTS `device_status` (
    `id`            INT AUTO_INCREMENT PRIMARY KEY,
    `fan_status`    TINYINT(1)   NOT NULL DEFAULT 0,
    `door_status`   TINYINT(1)   NOT NULL DEFAULT 0,
    `window_status` TINYINT(1)   NOT NULL DEFAULT 0,
    `buzzer_status` TINYINT(1)   NOT NULL DEFAULT 0,
    `led_state`     ENUM('SAFE','WARNING','DANGER') NOT NULL DEFAULT 'SAFE',
    `auto_mode`     TINYINT(1)   NOT NULL DEFAULT 1,
    `system_level`  ENUM('AN_TOAN','CANH_BAO','NGUY_HIEM') NOT NULL DEFAULT 'AN_TOAN',
    `last_message`  VARCHAR(100) NOT NULL DEFAULT 'HE THONG AN TOAN',
    `updated_at`    DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
                    ON UPDATE CURRENT_TIMESTAMP(3)
                    COMMENT 'Thoi diem cap nhat cuoi (ms precision)'
) ENGINE=InnoDB COMMENT='Snapshot trang thai thiet bi hien tai';
"""

# -----------------------------------------------------------
# 4. LICH SU CANH BAO
# -----------------------------------------------------------
SQL_ALERT_HISTORY = """
CREATE TABLE IF NOT EXISTS `alert_history` (
    `id`            BIGINT AUTO_INCREMENT PRIMARY KEY,
    `alert_type`    ENUM('GAS','TEMPERATURE','SECURITY','RAIN','COMBO') NOT NULL,
    `level`         ENUM('CANH_BAO','NGUY_HIEM') NOT NULL,
    `message`       VARCHAR(255) NOT NULL,
    `gas_value`     INT          DEFAULT NULL,
    `temperature`   FLOAT        DEFAULT NULL,
    `pir_status`    TINYINT(1)   DEFAULT NULL,
    `rain_status`   TINYINT(1)   DEFAULT NULL,
    `created_at`    DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
                    COMMENT 'Thoi diem phat sinh canh bao (ms precision)',
    `resolved_at`   DATETIME(3)  DEFAULT NULL
                    COMMENT 'Thoi diem xu ly xong canh bao (ms precision)',
    INDEX `idx_created_at` (`created_at`),
    INDEX `idx_alert_type` (`alert_type`),
    INDEX `idx_level` (`level`)
) ENGINE=InnoDB COMMENT='Lich su canh bao khi doc/nhiet do/an ninh';
"""

# -----------------------------------------------------------
# 5. LENH DIEU KHIEN - bo enum 'API'
# -----------------------------------------------------------
SQL_CONTROL_COMMANDS = """
CREATE TABLE IF NOT EXISTS `control_commands` (
    `id`            BIGINT AUTO_INCREMENT PRIMARY KEY,
    `source`        ENUM('WINFORM','MOBILE','TERMITE','SYSTEM') NOT NULL,
    `account_id`    INT          DEFAULT NULL,
    `command`       VARCHAR(50)  NOT NULL,
    `parameters`    VARCHAR(255) DEFAULT NULL,
    `status`        ENUM('PENDING','SENT','SUCCESS','FAILED') NOT NULL DEFAULT 'PENDING',
    `response`      VARCHAR(255) DEFAULT NULL,
    `sent_at`       DATETIME(3)  DEFAULT NULL
                    COMMENT 'Thoi diem gui lenh xuong Arduino (ms precision)',
    `responded_at`  DATETIME(3)  DEFAULT NULL
                    COMMENT 'Thoi diem nhan phan hoi (ms precision)',
    `created_at`    DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
                    COMMENT 'Thoi diem tao lenh (ms precision)',
    FOREIGN KEY (`account_id`) REFERENCES `accounts`(`id`) ON DELETE SET NULL,
    INDEX `idx_source` (`source`),
    INDEX `idx_command` (`command`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB COMMENT='Lich su lenh dieu khien tu WinForm/Termite';
"""

# -----------------------------------------------------------
# 6. LICH SU MO CUA - bo ip_address va password_len
# -----------------------------------------------------------
SQL_DOOR_ACCESS_LOG = """
CREATE TABLE IF NOT EXISTS `door_access_log` (
    `id`            BIGINT AUTO_INCREMENT PRIMARY KEY,
    `input_source`  ENUM('KEYPAD','TERMITE','WINFORM','MOBILE') NOT NULL DEFAULT 'KEYPAD',
    `role_matched`  ENUM('admin','user','none') NOT NULL DEFAULT 'none',
    `result`        ENUM('SUCCESS','FAILED','LOCKED') NOT NULL,
    `wrong_count`   TINYINT      NOT NULL DEFAULT 0,
    `note`          VARCHAR(255) DEFAULT NULL,
    `opened_at`     DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
                    COMMENT 'Thoi diem mo cua / truy cap (ms precision)',
    `closed_at`     DATETIME(3)  DEFAULT NULL
                    COMMENT 'Thoi diem dong cua (NULL = chua dong)',
    `duration_sec`  FLOAT        GENERATED ALWAYS AS
                    (TIMESTAMPDIFF(SECOND, `opened_at`, `closed_at`)) STORED
                    COMMENT 'Thoi gian cua mo (giay, tu dong tinh)',
    INDEX `idx_opened_at` (`opened_at`),
    INDEX `idx_result` (`result`),
    INDEX `idx_input_source` (`input_source`)
) ENGINE=InnoDB COMMENT='Lich su mo cua chinh bang keypad/passkey';
"""

# -----------------------------------------------------------
# 7. CAU HINH NGUONG
# -----------------------------------------------------------
SQL_THRESHOLD_CONFIG = """
CREATE TABLE IF NOT EXISTS `threshold_config` (
    `id`              INT AUTO_INCREMENT PRIMARY KEY,
    `config_key`      VARCHAR(50)  NOT NULL UNIQUE,
    `config_value`    VARCHAR(100) NOT NULL,
    `data_type`       ENUM('INT','FLOAT','BOOL','STRING') NOT NULL DEFAULT 'INT',
    `description`     VARCHAR(255) DEFAULT NULL,
    `updated_by`      INT          DEFAULT NULL,
    `updated_at`      DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
                      ON UPDATE CURRENT_TIMESTAMP(3)
                      COMMENT 'Thoi diem chinh sua cuoi (ms precision)',
    FOREIGN KEY (`updated_by`) REFERENCES `accounts`(`id`) ON DELETE SET NULL
) ENGINE=InnoDB COMMENT='Cau hinh nguong canh bao va tham so he thong';
"""


# ============================================================
# DU LIEU MAC DINH
# ============================================================

DEFAULT_ACCOUNTS = [
    # (username, plaintext_password, role, full_name)
    ("admin", "1234", "admin", "Quan Tri Vien"),
    ("user1", "2580", "user", "Nguoi Dung 1"),
    ("viewer", "0000", "viewer", "Nguoi Xem"),
]

DEFAULT_THRESHOLDS = [
    ("GAS_WARNING", "300", "INT", "Nguong canh bao khi MQ-3 (ADC)"),
    ("GAS_DANGER", "600", "INT", "Nguong nguy hiem khi MQ-3 (ADC)"),
    ("GAS_COMBO", "400", "INT", "Nguong khi + co nguoi thi nguy hiem (ADC)"),
    ("TEMP_WARNING", "35.0", "FLOAT", "Nguong canh bao nhiet do (do C)"),
    ("TEMP_DANGER", "40.0", "FLOAT", "Nguong nguy hiem nhiet do (do C)"),
    ("SEND_INTERVAL", "1000", "INT", "Chu ky gui du lieu len PC (ms)"),
    ("DOOR_OPEN_TIME", "2000", "INT", "Thoi gian mo cua (ms)"),
    ("DOOR_HOLD_TIME", "5000", "INT", "Thoi gian giu cua mo (ms)"),
    ("MAX_WRONG_PWD", "3", "INT", "So lan sai mat khau toi da"),
    ("AUTO_MODE", "1", "BOOL", "Che do tu dong mac dinh (1=bat, 0=tat)"),
]

# Seed du lieu de demo/kiem tra giao dien
# Luu y: script chi INSERT neu bang dang con rong/hoac khong co key duy nhat.
DEFAULT_SENSOR_DATA = [
    # (gas_value, temperature, pir_status, rain_status, system_level, recorded_at)
    # recorded_at se duoc gan theo thoi gian hien tai trong ham seed.
    (280, 33.5, 0, 0, "AN_TOAN", 0),
    (320, 34.8, 0, 1, "CANH_BAO", -10),
    (650, 41.2, 1, 0, "NGUY_HIEM", -20),
]

DEFAULT_DEVICE_STATUS = [
    # (id, fan_status, door_status, window_status, buzzer_status, led_state, auto_mode, system_level, last_message)
    (1, 0, 0, 0, 0, "SAFE", 1, "AN_TOAN", "HE THONG AN TOAN"),
]

DEFAULT_ALERT_HISTORY = [
    # (alert_type, level, message, gas_value, temperature, pir_status, rain_status, created_offset_min, resolved_offset_min)
    ("GAS", "CANH_BAO", "MQ-3 tang cao - canh bao khi MQ-3 vuot nguong", 320, None, 0, 1, -15, None),
    ("TEMPERATURE", "NGUY_HIEM", "Nhiet do vuot nguong nguy hiem - kich hoat canh bao", None, 41.2, 1, 0, -8, -3),
    ("SECURITY", "CANH_BAO", "Phat hien nguoi trong khu vuc - theo doi trang thai", None, None, 1, 0, -6, None),
]

DEFAULT_CONTROL_COMMANDS = [
    # (source, account_username, command, parameters, status, response, sent_offset_min, responded_offset_min)
    ("WINFORM", "admin", "SET_AUTO_MODE", "1", "SUCCESS", "OK", -25, -20),
    ("MOBILE", "user1", "OPEN_DOOR", "2000", "SENT", None, -12, None),
    ("SYSTEM", "viewer", "BUZZER_ON", "1", "FAILED", "TIMEOUT", -7, -6),
]

DEFAULT_DOOR_ACCESS_LOG = [
    # (input_source, role_matched, result, wrong_count, note, opened_offset_min, closed_offset_min)
    ("KEYPAD", "user", "SUCCESS", 0, "Mo cua thanh cong", -40, -38),
    ("TERMITE", "none", "FAILED", 2, "Sai mat khau / pin", -18, None),
    ("WINFORM", "admin", "LOCKED", 3, "Khoa tam thoi do nhieu lan sai", -5, None),
]


# ============================================================
# HAM TIEN ICH
# ============================================================

def get_connection(database: str | None = None):
    cfg = dict(DB_CONFIG)
    if database:
        cfg["database"] = database
    return mysql.connector.connect(**cfg)


def hash_password(plain: str) -> str:
    return bcrypt.hashpw(plain.encode("utf-8"), bcrypt.gensalt()).decode("utf-8")


def _utc_now_ms() -> datetime:
    # mysql dung DATETIME(3) => can day thoi gian co phan ms
    return datetime.now(timezone.utc)


def _format_mysql_dt(dt: datetime | None) -> str | None:
    if dt is None:
        return None
    # 'YYYY-MM-DD HH:MM:SS.mmm'
    return dt.strftime("%Y-%m-%d %H:%M:%S.") + f"{int(dt.microsecond/1000):03d}"


def _get_account_id_by_username(cur, username: str) -> int | None:
    cur.execute("SELECT id FROM `accounts` WHERE `username`=%s", (username,))
    row = cur.fetchone()
    if not row:
        return None
    return int(row["id"])


# ============================================================
# MIGRATE: bo cot du thua neu DB cu dang ton tai
# ============================================================

def _migrate_existing(cur, conn):
    """Xoa cac cot cu neu con ton tai (upgrade tu phien ban truoc)."""

    def col_exists(table, col):
        cur.execute(
            "SELECT COUNT(*) as c FROM information_schema.COLUMNS "
            "WHERE TABLE_SCHEMA=%s AND TABLE_NAME=%s AND COLUMN_NAME=%s",
            (DB_NAME, table, col),
        )
        return cur.fetchone()["c"] > 0

    # Bo email khoi accounts
    if col_exists("accounts", "email"):
        cur.execute("ALTER TABLE `accounts` DROP COLUMN `email`")
        conn.commit()
        print("  [MIGRATE] Da xoa cot 'accounts.email' (khong dung).")

    # Bo ip_address va password_len khoi door_access_log
    for col in ("ip_address", "password_len"):
        if col_exists("door_access_log", col):
            cur.execute(f"ALTER TABLE `door_access_log` DROP COLUMN `{col}`")
            conn.commit()
            print(f"  [MIGRATE] Da xoa cot 'door_access_log.{col}' (khong dung).")

    # Bo password_used neu con
    if col_exists("door_access_log", "password_used"):
        cur.execute("ALTER TABLE `door_access_log` DROP COLUMN `password_used`")
        conn.commit()
        print("  [MIGRATE] Da xoa cot 'door_access_log.password_used' (khong dung).")

    # Doi accessed_at -> opened_at neu con cu
    if col_exists("door_access_log", "accessed_at"):
        cur.execute(
            """
            ALTER TABLE `door_access_log`
            CHANGE `accessed_at` `opened_at`
            DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
            COMMENT 'Thoi diem mo cua / truy cap'
        """
        )
        conn.commit()
        print("  [MIGRATE] Da doi 'accessed_at' -> 'opened_at'.")


# ============================================================
# KHOI TAO CSDL
# ============================================================

def init_database():
    print("=" * 60)
    print("  KHOI TAO DATABASE: nha_thong_minh (v2 - toi uu)")
    print("=" * 60)

    # Buoc 1: Tao CSDL
    try:
        conn = get_connection()
        cur = conn.cursor()
        cur.execute(SQL_CREATE_DB)
        print(f"[OK] Database '{DB_NAME}' da san sang.")
        cur.close()
        conn.close()
    except Error as e:
        print(f"[LOI] Khong the ket noi MySQL: {e}")
        sys.exit(1)

    # Buoc 2: Tao cac bang + seed
    try:
        conn = get_connection(DB_NAME)
        cur = conn.cursor(dictionary=True)

        tables = [
            ("accounts", SQL_ACCOUNTS),
            ("sensor_data", SQL_SENSOR_DATA),
            ("device_status", SQL_DEVICE_STATUS),
            ("alert_history", SQL_ALERT_HISTORY),
            ("control_commands", SQL_CONTROL_COMMANDS),
            ("door_access_log", SQL_DOOR_ACCESS_LOG),
            ("threshold_config", SQL_THRESHOLD_CONFIG),
        ]

        for name, sql in tables:
            cur.execute(sql)
            print(f"[OK] Bang '{name}' da duoc tao (hoac da ton tai).")
        conn.commit()

        # Buoc 3: Migrate cot du thua neu DB cu
        print("\n[MIGRATE] Kiem tra va xoa cot du thua...")
        _migrate_existing(cur, conn)

        # Buoc 4: (KHONG SEED/UPDATE device_status)
        # Tran thai thiet bi se de luong Arduino/Proteus cap nhat qua serial bridge
        # de tranh viec ghi de lenh/dong bo khi cap nhat realtime.

        # Buoc 5: Tai khoan mac dinh
        sql_acc = """
            INSERT IGNORE INTO `accounts`
                (`username`, `password_hash`, `role`, `full_name`)
            VALUES (%s, %s, %s, %s)
        """
        for uname, pwd, role, full_name in DEFAULT_ACCOUNTS:
            hashed = hash_password(pwd)
            cur.execute(sql_acc, (uname, hashed, role, full_name))
            print(f"[OK] Tai khoan '{uname}' ({role}) da duoc them (neu chua co).")
        conn.commit()

        # Buoc 6: Nguong mac dinh
        sql_thr = """
            INSERT IGNORE INTO `threshold_config`
                (`config_key`, `config_value`, `data_type`, `description`)
            VALUES (%s, %s, %s, %s)
        """
        for row in DEFAULT_THRESHOLDS:
            cur.execute(sql_thr, row)
        conn.commit()
        print(f"[OK] {len(DEFAULT_THRESHOLDS)} nguong canh bao mac dinh da duoc them.")

        # -------------------------------------------------------
        # Buoc 7: Seed du lieu giai lap (neu bang con rong)
        # -------------------------------------------------------
        now = _utc_now_ms()

        # sensor_data: dam bao co it nhat 10 dong de UI co du data demo
        cur.execute("SELECT COUNT(*) as c FROM `sensor_data`")
        sensor_count = int(cur.fetchone()["c"])
        target_sensor_rows = 10
        if sensor_count < target_sensor_rows:
            sql_sensor = """
                INSERT INTO `sensor_data`
                    (`gas_value`, `temperature`, `pir_status`, `rain_status`, `system_level`, `recorded_at`)
                VALUES (%s, %s, %s, %s, %s, %s)
            """
            rows = []
            # lap lai pattern DEFAULT_SENSOR_DATA de tao nhieu dong (recorded_at khac nhau theo ms)
            pattern = list(DEFAULT_SENSOR_DATA)
            # bat dau offset sao cho recorded_at khong trung tu nhom cu (di tuong doi theo current now)
            base_offset_start = -30  # phu hop demo
            for i in range(target_sensor_rows - sensor_count):
                gas_value, temperature, pir_status, rain_status, system_level, _ = pattern[i % len(pattern)]
                dt = now + timedelta(minutes=base_offset_start - i)
                rows.append((gas_value, temperature, pir_status, rain_status, system_level, _format_mysql_dt(dt)))
            cur.executemany(sql_sensor, rows)
            conn.commit()
            print(f"[OK] Seed 'sensor_data' (count: {sensor_count} -> {target_sensor_rows}).")

        # device_status: chi seeding default khi chua co (tranh ghi de len trang thai da dieu khien/luu tru)
        cur.execute("SELECT COUNT(*) as c FROM `device_status` WHERE `id`=1")
        if int(cur.fetchone()["c"]) == 0:
            cur.execute(
                """
                INSERT INTO `device_status`
                    (`id`, `fan_status`, `door_status`, `window_status`, `buzzer_status`,
                     `led_state`, `auto_mode`, `system_level`, `last_message`)
                VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s)
                """,
                (
                    1,
                    DEFAULT_DEVICE_STATUS[0][1],
                    DEFAULT_DEVICE_STATUS[0][2],
                    DEFAULT_DEVICE_STATUS[0][3],
                    DEFAULT_DEVICE_STATUS[0][4],
                    DEFAULT_DEVICE_STATUS[0][5],
                    DEFAULT_DEVICE_STATUS[0][6],
                    DEFAULT_DEVICE_STATUS[0][7],
                    DEFAULT_DEVICE_STATUS[0][8],
                ),
            )
            conn.commit()
            print("[OK] Seed 'device_status' (id=1) theo default (vi chua co row).")

        # alert_history
        cur.execute("SELECT COUNT(*) as c FROM `alert_history`")
        alert_count = int(cur.fetchone()["c"])
        if alert_count == 0:
            sql_alert = """
                INSERT INTO `alert_history`
                    (`alert_type`, `level`, `message`, `gas_value`, `temperature`, `pir_status`, `rain_status`,
                     `created_at`, `resolved_at`)
                VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s)
            """
            rows = []
            for alert_type, level, message, gas_value, temperature, pir_status, rain_status, created_offset_min, resolved_offset_min in DEFAULT_ALERT_HISTORY:
                created_dt = now + timedelta(minutes=created_offset_min)
                resolved_dt = None if resolved_offset_min is None else now + timedelta(minutes=resolved_offset_min)
                rows.append(
                    (
                        alert_type,
                        level,
                        message,
                        gas_value,
                        temperature,
                        pir_status,
                        rain_status,
                        _format_mysql_dt(created_dt),
                        _format_mysql_dt(resolved_dt),
                    )
                )
            cur.executemany(sql_alert, rows)
            conn.commit()
            print("[OK] Seed 'alert_history' (bang dang rong).")

        # control_commands
        cur.execute("SELECT COUNT(*) as c FROM `control_commands`")
        cmd_count = int(cur.fetchone()["c"])
        if cmd_count == 0:
            sql_cmd = """
                INSERT INTO `control_commands`
                    (`source`, `account_id`, `command`, `parameters`, `status`, `response`, `sent_at`, `responded_at`, `created_at`)
                VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s)
            """
            rows = []
            for source, account_username, command, parameters, status, response, sent_offset_min, responded_offset_min in DEFAULT_CONTROL_COMMANDS:
                account_id = _get_account_id_by_username(cur, account_username)
                sent_dt = now + timedelta(minutes=sent_offset_min)
                responded_dt = None if responded_offset_min is None else now + timedelta(minutes=responded_offset_min)
                # created_at su dung ngay hien tai (hon sent_at), tinh thuc te khong qua quan trong cho demo
                created_dt = sent_dt - timedelta(seconds=5)
                rows.append(
                    (
                        source,
                        account_id,
                        command,
                        parameters,
                        status,
                        response,
                        _format_mysql_dt(sent_dt),
                        _format_mysql_dt(responded_dt),
                        _format_mysql_dt(created_dt),
                    )
                )
            cur.executemany(sql_cmd, rows)
            conn.commit()
            print("[OK] Seed 'control_commands' (bang dang rong).")

        # door_access_log
        cur.execute("SELECT COUNT(*) as c FROM `door_access_log`")
        door_count = int(cur.fetchone()["c"])
        if door_count == 0:
            sql_door = """
                INSERT INTO `door_access_log`
                    (`input_source`, `role_matched`, `result`, `wrong_count`, `note`, `opened_at`, `closed_at`)
                VALUES (%s, %s, %s, %s, %s, %s, %s)
            """
            rows = []
            for input_source, role_matched, result, wrong_count, note, opened_offset_min, closed_offset_min in DEFAULT_DOOR_ACCESS_LOG:
                opened_dt = now + timedelta(minutes=opened_offset_min)
                closed_dt = None if closed_offset_min is None else now + timedelta(minutes=closed_offset_min)
                rows.append(
                    (
                        input_source,
                        role_matched,
                        result,
                        wrong_count,
                        note,
                        _format_mysql_dt(opened_dt),
                        _format_mysql_dt(closed_dt),
                    )
                )
            cur.executemany(sql_door, rows)
            conn.commit()
            print("[OK] Seed 'door_access_log' (bang dang rong).")

        cur.close()
        conn.close()

        print("\n[HOAN THANH] Database da duoc khoi tao thanh cong!")
        print(f"  Host    : {DB_CONFIG['host']}:{DB_CONFIG['port']}")
        print(f"  Database: {DB_NAME}")
        print("=" * 60)

    except Error as e:
        print(f"[LOI] {e}")
        sys.exit(1)


# ============================================================
# MAIN
# ============================================================
if __name__ == "__main__":
    init_database()
