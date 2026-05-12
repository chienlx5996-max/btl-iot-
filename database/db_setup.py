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

# ============================================================
# CAU HINH KET NOI
# ============================================================
DB_CONFIG = {
    "host":     "localhost",
    "port":     3306,
    "user":     "root",
    "password": "123456789@",
    "charset":  "utf8mb4",
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
    ("admin",  "1234", "admin", "Quan Tri Vien"),
    ("user1",  "2580", "user",  "Nguoi Dung 1"),
    ("viewer", "0000", "viewer","Nguoi Xem"),
]

DEFAULT_THRESHOLDS = [
    ("GAS_WARNING",    "300",  "INT",   "Nguong canh bao khi MQ-3 (ADC)"),
    ("GAS_DANGER",     "600",  "INT",   "Nguong nguy hiem khi MQ-3 (ADC)"),
    ("GAS_COMBO",      "400",  "INT",   "Nguong khi + co nguoi thi nguy hiem (ADC)"),
    ("TEMP_WARNING",   "35.0", "FLOAT", "Nguong canh bao nhiet do (do C)"),
    ("TEMP_DANGER",    "40.0", "FLOAT", "Nguong nguy hiem nhiet do (do C)"),
    ("SEND_INTERVAL",  "1000", "INT",   "Chu ky gui du lieu len PC (ms)"),
    ("DOOR_OPEN_TIME", "2000", "INT",   "Thoi gian mo cua (ms)"),
    ("DOOR_HOLD_TIME", "5000", "INT",   "Thoi gian giu cua mo (ms)"),
    ("MAX_WRONG_PWD",  "3",    "INT",   "So lan sai mat khau toi da"),
    ("AUTO_MODE",      "1",    "BOOL",  "Che do tu dong mac dinh (1=bat, 0=tat)"),
]


# ============================================================
# HAM TIEN ICH
# ============================================================

def get_connection(database: str = None):
    cfg = dict(DB_CONFIG)
    if database:
        cfg["database"] = database
    return mysql.connector.connect(**cfg)


def hash_password(plain: str) -> str:
    return bcrypt.hashpw(plain.encode("utf-8"), bcrypt.gensalt()).decode("utf-8")


# ============================================================
# MIGRATE: bo cot du thua neu DB cu dang ton tai
# ============================================================

def _migrate_existing(cur, conn):
    """Xoa cac cot cu neu con ton tai (upgrade tu phien ban truoc)."""
    def col_exists(table, col):
        cur.execute(
            "SELECT COUNT(*) as c FROM information_schema.COLUMNS "
            "WHERE TABLE_SCHEMA=%s AND TABLE_NAME=%s AND COLUMN_NAME=%s",
            (DB_NAME, table, col)
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
        cur.execute("""
            ALTER TABLE `door_access_log`
            CHANGE `accessed_at` `opened_at`
            DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
            COMMENT 'Thoi diem mo cua / truy cap'
        """)
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
        cur  = conn.cursor()
        cur.execute(SQL_CREATE_DB)
        print(f"[OK] Database '{DB_NAME}' da san sang.")
        cur.close()
        conn.close()
    except Error as e:
        print(f"[LOI] Khong the ket noi MySQL: {e}")
        sys.exit(1)

    # Buoc 2: Tao cac bang
    try:
        conn = get_connection(DB_NAME)
        cur  = conn.cursor(dictionary=True)

        tables = [
            ("accounts",         SQL_ACCOUNTS),
            ("sensor_data",      SQL_SENSOR_DATA),
            ("device_status",    SQL_DEVICE_STATUS),
            ("alert_history",    SQL_ALERT_HISTORY),
            ("control_commands", SQL_CONTROL_COMMANDS),
            ("door_access_log",  SQL_DOOR_ACCESS_LOG),
            ("threshold_config", SQL_THRESHOLD_CONFIG),
        ]

        for name, sql in tables:
            cur.execute(sql)
            print(f"[OK] Bang '{name}' da duoc tao (hoac da ton tai).")
        conn.commit()

        # Buoc 3: Migrate cot du thua neu DB cu
        print("\n[MIGRATE] Kiem tra va xoa cot du thua...")
        _migrate_existing(cur, conn)

        # Buoc 4: Row trang thai thiet bi
        cur.execute("INSERT IGNORE INTO `device_status` (`id`) VALUES (1)")
        conn.commit()
        print("[OK] Row trang thai thiet bi (id=1) san sang.")

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
