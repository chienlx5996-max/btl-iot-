# Smart Home IoT — Giám sát an toàn & tự động thông gió (Gas/Temperature/Rain/Security)

## 1) Tổng quan đề tài (đúng với repo)
Đây là mô hình **nhà thông minh ứng dụng IoT** nhằm:
- **Giám sát môi trường thời gian thực**: nồng độ **khí gas/cồn** (MQ-3), **nhiệt độ** (LM35), **trạng thái người** (PIR), **trạng thái mưa** (Rain Sensor).
- **Cảnh báo an toàn** khi vượt ngưỡng: cập nhật LED/Buzzer và hiển thị trạng thái trên màn hình LCD (tại mô hình Arduino).
- **Tự động hóa**: bật/tắt **quạt**, điều khiển **mở/đóng cửa sổ** tùy theo điều kiện môi trường (đặc biệt là khi có mưa).
- **Điều khiển từ xa qua Web dashboard** (Flask): người dùng có thể bật/tắt thiết bị hoặc bật **chế độ tự động**.
- **Lưu trữ & truy vết**: dữ liệu cảm biến, lịch sử cảnh báo, lịch sử lệnh điều khiển, lịch sử truy cập (door access log) được lưu trong MySQL.

## 2) Kiến trúc hệ thống (chuỗi hoạt động)
### 2.1. Arduino ↔ Serial
- Arduino đọc cảm biến và cập nhật LCD/LED/Buzzer theo logic tự động (`autoLogic`).
- Arduino **gửi dữ liệu sensor** lên PC qua Serial theo dạng chuỗi:
  - `STAT|GAS:...|TEMP:...|PIR:0|RAIN:0|MODE:A` (hoặc biến thể định dạng `KEY=VALUE;...`)
- Arduino **nhận lệnh điều khiển** từ PC qua Serial với các lệnh dạng text:
  - `FAN_ON`, `FAN_OFF`
  - `DOOR_OPEN`, `DOOR_CLOSE`
  - `WINDOW_OPEN`, `WINDOW_CLOSE`
  - `BUZZER_ON`, `BUZZER_OFF`

### 2.2. Web Flask ↔ MySQL (DB)
Backend Flask có các phần chính:
- **RBAC đăng nhập** bằng session: `admin`, `user`, `viewer`.
- **API lấy dữ liệu trạng thái**:
  - `GET /api/status` (lấy latest sensor + snapshot device_status + số lượng alert đang hoạt động)
- **API điều khiển**:
  - `POST /api/control` (ghi lệnh vào `control_commands` và cập nhật `device_status`)
- **API lịch sử & nhật ký**:
  - `GET /api/logs/sensors`
  - `GET /api/logs/alerts`
  - `GET /api/logs/commands`
  - `GET /api/logs/door-access` (có filter result)
- **API admin**:
  - `GET /api/alerts/active`
  - `POST /api/alerts/resolve`
  - `GET /api/thresholds`
  - `POST /api/thresholds/update`
  - `GET /api/accounts`, `POST /api/accounts/add`, `POST /api/accounts/deactivate`, `POST /api/accounts/change-password`

### 2.3. Serial bridge trong Flask
Trong `btl-iot-/webapp/app.py`:
- Flask chạy background thread để **đọc lệnh PENDING** trong MySQL bảng `control_commands`
- Gửi tương ứng xuống Arduino qua COM (Serial)
- Lệnh có thể được mở rộng khi cần (ví dụ `AUTO_SHUTDOWN` sẽ chuyển sang set các lệnh con)

> Lưu ý quan trọng về phản hồi (ACK):
> - Ở phiên bản hiện tại, **Arduino đã nhận và thực thi lệnh**, nhưng **chưa gửi lại chuỗi ACK/response** về PC (ví dụ dạng `LENH=...|KET_QUA=OK`).
> - Vì vậy, trạng thái lệnh trong DB chủ yếu phản ánh **quá trình “đã gửi xuống serial (SENT/TX)”**, **chưa thể kết luận “thực thi thành công”** dựa trên phản hồi từ Arduino.
>
> Ý nghĩa tổng thể: Web dashboard không điều khiển trực tiếp thiết bị; thay vào đó, mọi lệnh đi qua MySQL → Serial bridge → Arduino. Điều này giúp có lịch sử, phân quyền và truy vết.

## 3) Cấu trúc dữ liệu MySQL (theo `schema.sql`)
Các bảng chính:
- `accounts`: tài khoản + phân quyền (`admin`, `user`, `viewer`)
- `sensor_data`: dữ liệu cảm biến theo thời gian
  - `gas_value`, `temperature`, `pir_status`, `rain_status`, `system_level`, `recorded_at`
- `device_status` (id=1): snapshot trạng thái thiết bị hiện tại + `auto_mode` + `system_level` + `last_message`
- `alert_history`: lịch sử cảnh báo (CANH_BAO/NGUY_HIEM), có `resolved_at`
- `control_commands`: lịch sử lệnh điều khiển (PENDING/SENT/SUCCESS/FAILED)
- `door_access_log`: lịch sử mở cửa/verify theo keypad (SUCCESS/FAILED/LOCKED)
- `threshold_config`: cấu hình ngưỡng cảnh báo (GAS_WARNING, GAS_DANGER, TEMP_WARNING, TEMP_DANGER, ...)

## 4) Logic cảnh báo & ngưỡng (theo DBHelper)
Trong `btl-iot-/database/db_helper.py`:
- Dữ liệu từ Arduino được parse và đưa vào `sensor_data`
- `system_level` được tính bằng ngưỡng từ `threshold_config`:
  - `NGUY_HIEM` nếu `gas >= GAS_DANGER` hoặc `temp >= TEMP_DANGER`
  - `CANH_BAO` nếu `gas >= GAS_WARNING` hoặc `temp >= TEMP_WARNING`
  - ngược lại `AN_TOAN`
- Nếu có cảnh báo và bật `auto_alert`, hệ thống sẽ tạo bản ghi trong `alert_history`.

## 5) Giao diện web (btl-iot-/webapp/)
Các trang chính:
- `GET /` → Dashboard hiển thị sensor + điều khiển thiết bị
- `GET /admin` → Trang cấu hình ngưỡng, resolve alert, quản lý accounts
- `GET /history` → Trang lịch sử cảm biến/cảnh báo/lệnh/truy cập

Frontend:
- `templates/index.html`, `templates/admin.html`, `templates/history.html`
- `static/js/main.js`: gọi `updateStatus()`, `loadLogs()`, và gửi control qua `/api/control`
- `static/css/style.css`: theme giao diện (brutalist, không gradient theo thiết kế hiện tại)

## 6) Cách chạy (mô phỏng Proteus/Arduino + Web)
1. Chuẩn bị MySQL và chạy schema trong `btl-iot-/database/schema.sql`
2. Cài Python dependencies (xem `requirements.txt` của dự án nếu có)
3. Kết nối Arduino/Proteus serial và đảm bảo COM port đúng:
   - Trong `webapp/app.py` hiện đang dùng `SERIAL_PORT = "COM2"` (có thể cần đổi cho đúng máy)
4. Khởi chạy Flask:
   - Mặc định chạy ở port `5000`
5. Mở trình duyệt và đăng nhập:
   - URL: `http://localhost:5000/`
6. Trên Dashboard:
   - xem dữ liệu realtime qua `GET /api/status`
   - điều khiển qua `POST /api/control`
   - xem lịch sử qua `/history`

## 7) Điểm nổi bật “đúng chủ đề IoT”
- **Realtime monitoring**: cảm biến → Serial → DB → Web
- **Closed-loop control**: lệnh điều khiển được phản ánh về snapshot (`device_status`) và đi xuống Arduino
- **Safety automation**: autoLogic trên Arduino kết hợp rule ngưỡng
- **RBAC & audit trail**: admin/user/viewer + nhật ký lệnh + lịch sử cảnh báo
- **Configurable thresholds**: admin chỉnh `threshold_config` và áp dụng tức thì

## 8) Tương thích với nội dung mô hình phần cứng
Code Arduino trong `btl-iot-/code.ino` khớp với các thành phần mô tả:
- Cảm biến: MQ-3, LM35, PIR, Rain sensor
- Cơ cấu: quạt (FAN), cửa sổ (WINDOW), cửa chính (DOOR) + LCD/LED/Buzzer
- Chuỗi dữ liệu Serial và các lệnh text điều khiển.
