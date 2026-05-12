-- =============================================================
--  DO AN IOT NHA THONG MINH
--  Schema MySQL - Toi uu hoa
--  Cap nhat: 2026-05-12
--  
--  Thay doi so voi phien ban cu:
--    - accounts  : Bo `email` (khong dung trong UI)
--    - control_commands : Bo enum 'API' khoi source (da xoa api_server.py)
--    - door_access_log  : Bo `ip_address`, `password_len` (luon NULL)
-- =============================================================

CREATE DATABASE IF NOT EXISTS `nha_thong_minh`
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE `nha_thong_minh`;

-- ------------------------------------------------------------
-- 1. TAI KHOAN VA PHAN QUYEN
-- ------------------------------------------------------------
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

-- ------------------------------------------------------------
-- 2. DU LIEU CAM BIEN THEO THOI GIAN
-- ------------------------------------------------------------
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
) ENGINE=InnoDB COMMENT='Du lieu cam bien MQ-3/LM35/PIR/Rain theo thoi gian';

-- ------------------------------------------------------------
-- 3. TRANG THAI HIEN TAI THIET BI (1 row duy nhat id=1)
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `device_status` (
    `id`            INT AUTO_INCREMENT PRIMARY KEY,
    `fan_status`    TINYINT(1)   NOT NULL DEFAULT 0 COMMENT '1=Bat',
    `door_status`   TINYINT(1)   NOT NULL DEFAULT 0 COMMENT '1=Mo',
    `window_status` TINYINT(1)   NOT NULL DEFAULT 0 COMMENT '1=Mo',
    `buzzer_status` TINYINT(1)   NOT NULL DEFAULT 0 COMMENT '1=Bat',
    `led_state`     ENUM('SAFE','WARNING','DANGER') NOT NULL DEFAULT 'SAFE',
    `auto_mode`     TINYINT(1)   NOT NULL DEFAULT 1  COMMENT '1=Tu dong',
    `system_level`  ENUM('AN_TOAN','CANH_BAO','NGUY_HIEM') NOT NULL DEFAULT 'AN_TOAN',
    `last_message`  VARCHAR(100) NOT NULL DEFAULT 'HE THONG AN TOAN',
    `updated_at`    DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
                    ON UPDATE CURRENT_TIMESTAMP(3)
                    COMMENT 'Thoi diem cap nhat cuoi (ms precision)'
) ENGINE=InnoDB COMMENT='Snapshot trang thai thiet bi hien tai (upsert row id=1)';

INSERT IGNORE INTO `device_status` (`id`) VALUES (1);

-- ------------------------------------------------------------
-- 4. LICH SU CANH BAO
-- ------------------------------------------------------------
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
                    COMMENT 'Thoi diem xu ly xong canh bao',
    INDEX `idx_created_at` (`created_at`),
    INDEX `idx_alert_type` (`alert_type`),
    INDEX `idx_level` (`level`)
) ENGINE=InnoDB COMMENT='Lich su canh bao khi doc/nhiet do/an ninh';

-- ------------------------------------------------------------
-- 5. LENH DIEU KHIEN (bo enum API - da xoa api_server.py)
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `control_commands` (
    `id`            BIGINT AUTO_INCREMENT PRIMARY KEY,
    `source`        ENUM('WINFORM','MOBILE','TERMITE','SYSTEM') NOT NULL
                    COMMENT 'Nguon gui lenh',
    `account_id`    INT          DEFAULT NULL COMMENT 'Nguoi gui (NULL=he thong)',
    `command`       VARCHAR(50)  NOT NULL COMMENT 'FAN_ON, DOOR_OPEN, ...',
    `parameters`    VARCHAR(255) DEFAULT NULL,
    `status`        ENUM('PENDING','SENT','SUCCESS','FAILED') NOT NULL DEFAULT 'PENDING',
    `response`      VARCHAR(255) DEFAULT NULL,
    `sent_at`       DATETIME(3)  DEFAULT NULL
                    COMMENT 'Thoi diem gui lenh xuong Arduino',
    `responded_at`  DATETIME(3)  DEFAULT NULL
                    COMMENT 'Thoi diem nhan phan hoi tu Arduino',
    `created_at`    DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
                    COMMENT 'Thoi diem tao lenh (ms precision)',
    FOREIGN KEY (`account_id`) REFERENCES `accounts`(`id`) ON DELETE SET NULL,
    INDEX `idx_source` (`source`),
    INDEX `idx_command` (`command`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB COMMENT='Lich su lenh dieu khien tu WinForm/Termite';

-- ------------------------------------------------------------
-- 6. LICH SU MO CUA BANG KEYPAD
--    Bo: ip_address (luon NULL voi KEYPAD), password_len (khong dung)
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `door_access_log` (
    `id`            BIGINT AUTO_INCREMENT PRIMARY KEY,
    `input_source`  ENUM('KEYPAD','TERMITE','WINFORM','MOBILE') NOT NULL DEFAULT 'KEYPAD',
    `role_matched`  ENUM('admin','user','none') NOT NULL DEFAULT 'none'
                    COMMENT 'Quyen tuong ung mat khau dung',
    `result`        ENUM('SUCCESS','FAILED','LOCKED') NOT NULL,
    `wrong_count`   TINYINT      NOT NULL DEFAULT 0,
    `note`          VARCHAR(255) DEFAULT NULL,
    `opened_at`     DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
                    COMMENT 'Thoi diem mo cua / truy cap (ms precision)',
    `closed_at`     DATETIME(3)  DEFAULT NULL
                    COMMENT 'Thoi diem dong cua (NULL = chua dong)',
    `duration_sec`  FLOAT        GENERATED ALWAYS AS
                    (TIMESTAMPDIFF(SECOND, `opened_at`, `closed_at`)) STORED
                    COMMENT 'Thoi gian cua mo tinh tu ms (tu dong tinh)',
    INDEX `idx_opened_at` (`opened_at`),
    INDEX `idx_result` (`result`),
    INDEX `idx_input_source` (`input_source`)
) ENGINE=InnoDB COMMENT='Lich su mo cua chinh bang keypad/passkey';

-- ------------------------------------------------------------
-- 7. CAU HINH NGUONG CANH BAO
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `threshold_config` (
    `id`              INT AUTO_INCREMENT PRIMARY KEY,
    `config_key`      VARCHAR(50)  NOT NULL UNIQUE,
    `config_value`    VARCHAR(100) NOT NULL,
    `data_type`       ENUM('INT','FLOAT','BOOL','STRING') NOT NULL DEFAULT 'INT',
    `description`     VARCHAR(255) DEFAULT NULL,
    `updated_by`      INT          DEFAULT NULL COMMENT 'account_id nguoi chinh sua',
    `updated_at`      DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
                      ON UPDATE CURRENT_TIMESTAMP(3)
                      COMMENT 'Thoi diem chinh sua cuoi (ms precision)',
    FOREIGN KEY (`updated_by`) REFERENCES `accounts`(`id`) ON DELETE SET NULL
) ENGINE=InnoDB COMMENT='Cau hinh nguong canh bao va tham so he thong';

INSERT IGNORE INTO `threshold_config` (`config_key`,`config_value`,`data_type`,`description`) VALUES
    ('GAS_WARNING',    '300',   'INT',   'Nguong canh bao khi MQ-3 (ADC)'),
    ('GAS_DANGER',     '600',   'INT',   'Nguong nguy hiem khi MQ-3 (ADC)'),
    ('GAS_COMBO',      '400',   'INT',   'Nguong khi + co nguoi = nguy hiem (ADC)'),
    ('TEMP_WARNING',   '35.0',  'FLOAT', 'Nguong canh bao nhiet do (do C)'),
    ('TEMP_DANGER',    '40.0',  'FLOAT', 'Nguong nguy hiem nhiet do (do C)'),
    ('SEND_INTERVAL',  '1000',  'INT',   'Chu ky gui du lieu len PC (ms)'),
    ('DOOR_OPEN_TIME', '2000',  'INT',   'Thoi gian mo cua (ms)'),
    ('DOOR_HOLD_TIME', '5000',  'INT',   'Thoi gian giu cua mo (ms)'),
    ('MAX_WRONG_PWD',  '3',     'INT',   'So lan sai mat khau toi da'),
    ('AUTO_MODE',      '1',     'BOOL',  'Che do tu dong mac dinh (1=bat)');
