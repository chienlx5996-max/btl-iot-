"""
=============================================================
  DB MIGRATION - Cap nhat schema len phien ban moi nhat
  
  Chay script nay de:
    - Cap nhat bang door_access_log (accessed_at -> opened_at + closed_at)
    - Kiem tra toan bo cau truc bang
    - Khong mat du lieu cu
    
  Chay:
    python db_migrate.py
=============================================================
"""

import mysql.connector
from mysql.connector import Error
import sys

DB_CONFIG = {
    "host":     "localhost",
    "port":     3306,
    "user":     "root",        # doi thanh user cua ban
    "password": "123456789@",            # doi thanh password cua ban
    "database": "nha_thong_minh",
    "charset":  "utf8mb4",
}


def get_conn():
    return mysql.connector.connect(**DB_CONFIG)


def column_exists(cur, table: str, col: str) -> bool:
    cur.execute(
        "SELECT COUNT(*) as cnt FROM information_schema.COLUMNS "
        "WHERE TABLE_SCHEMA=%s AND TABLE_NAME=%s AND COLUMN_NAME=%s",
        (DB_CONFIG["database"], table, col)
    )
    return cur.fetchone()["cnt"] > 0


def table_exists(cur, table: str) -> bool:
    cur.execute(
        "SELECT COUNT(*) as cnt FROM information_schema.TABLES "
        "WHERE TABLE_SCHEMA=%s AND TABLE_NAME=%s",
        (DB_CONFIG["database"], table)
    )
    return cur.fetchone()["cnt"] > 0


# ============================================================
def migrate():
    print("=" * 60)
    print("  DB MIGRATION - NHA THONG MINH")
    print("=" * 60)

    try:
        conn = get_conn()
        cur  = conn.cursor(dictionary=True)
        print("[OK] Ket noi MySQL thanh cong.\n")
    except Error as e:
        print(f"[LOI] Khong ket noi duoc MySQL: {e}")
        print("      Kiem tra lai host/user/password trong file nay.")
        sys.exit(1)

    # ----------------------------------------------------------
    # 1. KIEM TRA VA TAO BANG CON THIEU
    # ----------------------------------------------------------
    print("--- [1] Kiem tra cac bang ---")

    # Tao bang neu chua co (dung CREATE TABLE IF NOT EXISTS)
    # Danh sach bang can co
    required_tables = [
        "accounts", "sensor_data", "device_status",
        "alert_history", "control_commands",
        "door_access_log", "threshold_config"
    ]
    for t in required_tables:
        exists = table_exists(cur, t)
        print(f"  {'[CO]' if exists else '[THIEU]'} {t}")

    conn.commit()

    # ----------------------------------------------------------
    # 2. MIGRATE door_access_log
    # ----------------------------------------------------------
    print("\n--- [2] Migrate bang door_access_log ---")

    if not table_exists(cur, "door_access_log"):
        print("  Bang chua ton tai, se tao moi...")
        cur.execute("""
            CREATE TABLE `door_access_log` (
                `id`            BIGINT AUTO_INCREMENT PRIMARY KEY,
                `input_source`  ENUM('KEYPAD','TERMITE','WINFORM','MOBILE') NOT NULL DEFAULT 'KEYPAD',
                `role_matched`  ENUM('admin','user','none') NOT NULL DEFAULT 'none',
                `result`        ENUM('SUCCESS','FAILED','LOCKED') NOT NULL,
                `wrong_count`   TINYINT      NOT NULL DEFAULT 0,
                `note`          VARCHAR(255) DEFAULT NULL,
                `opened_at`     DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
                `closed_at`     DATETIME(3)  DEFAULT NULL,
                `duration_sec`  FLOAT        GENERATED ALWAYS AS
                                (TIMESTAMPDIFF(SECOND, `opened_at`, `closed_at`)) STORED,
                INDEX `idx_opened_at` (`opened_at`),
                INDEX `idx_result` (`result`),
                INDEX `idx_input_source` (`input_source`)
            ) ENGINE=InnoDB COMMENT='Lich su mo cua chinh bang keypad/passkey'
        """)
        conn.commit()
        print("  [OK] Da tao bang door_access_log moi.")

    else:
        # Kiem tra cot cu 'accessed_at' va doi sang moi
        if column_exists(cur, "door_access_log", "accessed_at"):
            print("  Phat hien cot cu 'accessed_at', dang chuyen doi...")

            # Doi ten cot accessed_at -> opened_at
            try:
                cur.execute("""
                    ALTER TABLE `door_access_log`
                    CHANGE `accessed_at` `opened_at`
                    DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
                    COMMENT 'Thoi diem mo cua thanh cong / truy cap'
                """)
                conn.commit()
                print("  [OK] Da doi 'accessed_at' -> 'opened_at'.")
            except Error as e:
                print(f"  [WARN] Khong the doi ten cot: {e}")

        # Them cot closed_at neu chua co
        if not column_exists(cur, "door_access_log", "closed_at"):
            try:
                cur.execute("""
                    ALTER TABLE `door_access_log`
                    ADD COLUMN `closed_at` DATETIME(3) DEFAULT NULL
                    COMMENT 'Thoi diem dong cua'
                """)
                conn.commit()
                print("  [OK] Da them cot 'closed_at'.")
            except Error as e:
                print(f"  [WARN] Khong them duoc 'closed_at': {e}")
        else:
            print("  [OK] Cot 'closed_at' da co san.")

        # Them cot duration_sec neu chua co
        if not column_exists(cur, "door_access_log", "duration_sec"):
            try:
                cur.execute("""
                    ALTER TABLE `door_access_log`
                    ADD COLUMN `duration_sec` FLOAT
                    GENERATED ALWAYS AS
                    (TIMESTAMPDIFF(SECOND, `opened_at`, `closed_at`)) STORED
                    COMMENT 'Thoi gian cua mo (giay)'
                """)
                conn.commit()
                print("  [OK] Da them cot 'duration_sec'.")
            except Error as e:
                print(f"  [WARN] Khong them duoc 'duration_sec': {e}")
        else:
            print("  [OK] Cot 'duration_sec' da co san.")

        # Xoa cot password_used neu con ton tai (bo trong phien ban moi)
        if column_exists(cur, "door_access_log", "password_used"):
            try:
                cur.execute("ALTER TABLE `door_access_log` DROP COLUMN `password_used`")
                conn.commit()
                print("  [OK] Da xoa cot 'password_used' (khong dung nua).")
            except Error as e:
                print(f"  [WARN] Khong xoa duoc 'password_used': {e}")

        print("  [OK] Bang door_access_log da cap nhat xong.")

    # ----------------------------------------------------------
    # 3. DAM BAO device_status CO ROW ID=1
    # ----------------------------------------------------------
    print("\n--- [3] Kiem tra device_status row id=1 ---")
    cur.execute("SELECT COUNT(*) as cnt FROM `device_status` WHERE `id` = 1")
    if cur.fetchone()["cnt"] == 0:
        cur.execute("INSERT INTO `device_status` (`id`) VALUES (1)")
        conn.commit()
        print("  [OK] Da them row trang thai thiet bi (id=1).")
    else:
        print("  [OK] Row trang thai da ton tai.")

    # ----------------------------------------------------------
    # 4. KIEM TRA CUOI
    # ----------------------------------------------------------
    print("\n--- [4] Kiem tra INSERT thu vao door_access_log ---")
    try:
        cur.execute("""
            INSERT INTO `door_access_log`
                (`input_source`, `role_matched`, `result`, `wrong_count`, `note`)
            VALUES ('KEYPAD', 'none', 'FAILED', 1, 'Migration test - co the xoa')
        """)
        conn.commit()
        test_id = cur.lastrowid
        # Xoa luon ban ghi test
        cur.execute("DELETE FROM `door_access_log` WHERE `id` = %s", (test_id,))
        conn.commit()
        print("  [OK] INSERT test thanh cong - schema dung.\n")
    except Error as e:
        print(f"  [LOI] INSERT test that bai: {e}")
        print("        Schema co the van con van de.\n")

    # ----------------------------------------------------------
    # 5. KIEM TRA INSERT thu vao sensor_data
    # ----------------------------------------------------------
    print("--- [5] Kiem tra INSERT thu vao sensor_data ---")
    try:
        cur.execute("""
            INSERT INTO `sensor_data`
                (`gas_value`, `temperature`, `pir_status`, `rain_status`, `system_level`)
            VALUES (0, 0.0, 0, 0, 'AN_TOAN')
        """)
        conn.commit()
        test_id = cur.lastrowid
        cur.execute("DELETE FROM `sensor_data` WHERE `id` = %s", (test_id,))
        conn.commit()
        print("  [OK] INSERT sensor_data test thanh cong.\n")
    except Error as e:
        print(f"  [LOI] INSERT sensor_data that bai: {e}\n")

    cur.close()
    conn.close()

    print("=" * 60)
    print("  MIGRATION HOAN THANH!")
    print("  Bay gio chay lai: python serial_reader.py")
    print("=" * 60)


if __name__ == "__main__":
    migrate()
