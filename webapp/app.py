import os
import sys
import threading
import time
from flask import Flask, render_template, request, jsonify, session, redirect, url_for
from datetime import datetime, timezone, timedelta

# Múi giờ Việt Nam (UTC+7)
VIETNAM_TZ = timezone(timedelta(hours=7))

# Thêm thư mục gốc vào path để import database
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..', 'btl-iot-', 'database')))

try:
    from database.db_helper import DBHelper
except ImportError:
    try:
        from db_helper import DBHelper
    except ImportError:
        print("[ERROR] Cannot import DBHelper from database folder")
        raise

app = Flask(__name__)
app.secret_key = 'smart_home_secret_key'  # Thay đổi trong thực tế
db = DBHelper()

# ============================================================
# SERIAL BRIDGE (gộp serial_reader vào cùng process Flask)
# ============================================================
# NOTE: serial_reader.py sẽ chạy RIÊNG như daemon process
# KHÔNG chạy serial thread trong Flask nữa để tránh conflict
SERIAL_PORT = "COM2"
SERIAL_BAUD = 9600
_SERIAL_THREAD_STARTED = False


# ============================================================
# Middleware
# ============================================================
def is_logged_in():
    return 'user_id' in session


def require_login():
    if not is_logged_in():
        return jsonify({'error': 'Unauthorized'}), 401
    return None


def require_admin():
    if not is_logged_in():
        return jsonify({'error': 'Unauthorized'}), 401
    if session.get('role') != 'admin':
        return jsonify({'error': 'Forbidden'}), 403
    return None


# ============================================================
# Auth
# ============================================================
@app.route('/login', methods=['GET', 'POST'])
def login():
    if request.method == 'POST':
        data = request.get_json(force=True) or {}
        username = data.get('username', '').strip()
        password = data.get('password', '').strip()

        user = db.verify_login(username, password)
        if user:
            session['user_id'] = user['id']
            session['username'] = user['username']
            session['role'] = user['role']
            return jsonify({'success': True})
        return jsonify({'success': False, 'message': 'Sai tài khoản hoặc mật khẩu'})

    return render_template('login.html')


@app.route('/logout')
def logout():
    session.clear()
    return redirect(url_for('login'))


# ============================================================
# Pages
# ============================================================
@app.route('/')
def index():
    if not is_logged_in():
        return redirect(url_for('login'))
    return render_template('index.html', user=session)


@app.route('/admin')
def admin_page():
    admin_err = require_admin()
    if admin_err:
        return admin_err
    return render_template('admin.html', user=session)


@app.route('/history')
def history_page():
    if not is_logged_in():
        return redirect(url_for('login'))
    return render_template('history.html', user=session)


# ============================================================
# API: Status / Control / Logs (existing)
# ============================================================
@app.route('/api/status')
def get_status():
    login_err = require_login()
    if login_err:
        return login_err

    sensor = db.get_latest_sensor()
    device = db.get_device_status()
    alerts = db.get_active_alerts()

    return jsonify({
        'sensor': sensor,
        'device': device,
        'alerts_count': len(alerts)
    })


@app.route('/api/control', methods=['POST'])
def control_device():
    login_err = require_login()
    if login_err:
        return login_err

    if session.get('role') == 'viewer':
        return jsonify({'success': False, 'message': 'Bạn không có quyền điều khiển'})

    data = request.get_json(force=True) or {}
    action = data.get('action')  # e.g., 'FAN_ON', 'DOOR_OPEN'

    status_update = {}
    if action == 'FAN_ON':
        status_update = {'fan_status': 1}
    elif action == 'FAN_OFF':
        status_update = {'fan_status': 0}
    elif action == 'DOOR_OPEN':
        status_update = {'door_status': 1}
    elif action == 'DOOR_CLOSE':
        status_update = {'door_status': 0}
    elif action == 'WINDOW_OPEN':
        status_update = {'window_status': 1}
    elif action == 'WINDOW_CLOSE':
        status_update = {'window_status': 0}
    elif action == 'BUZZER_ON':
        status_update = {'buzzer_status': 1}
    elif action == 'BUZZER_OFF':
        status_update = {'buzzer_status': 0}
    elif action == 'MODE_AUTO':
        status_update = {'auto_mode': 1}
    elif action == 'MODE_MANUAL':
        status_update = {'auto_mode': 0}

    try:
        db.insert_command(source='MOBILE', command=action, account_id=session['user_id'])
        if status_update:
            db.update_device_status(**status_update)
        return jsonify({'success': True})
    except Exception as e:
        return jsonify({'success': False, 'message': str(e)})


@app.route('/api/logs/sensors')
def get_sensor_logs():
    logs = db.get_sensor_data(limit=50)
    return jsonify(logs)


@app.route('/api/logs/alerts')
def get_alert_logs():
    logs = db.get_alert_history(limit=50)
    return jsonify(logs)


# ============================================================
# API: Alerts (admin)
# ============================================================
@app.route('/api/alerts/active')
def get_active_alerts():
    login_err = require_login()
    if login_err:
        return login_err
    alerts = db.get_active_alerts()
    return jsonify(alerts)


@app.route('/api/alerts/resolve', methods=['POST'])
def resolve_alerts():
    admin_err = require_admin()
    if admin_err:
        return admin_err

    data = request.get_json(force=True) or {}
    alert_id = data.get('alert_id')
    if not alert_id:
        return jsonify({'success': False, 'message': 'Missing alert_id'}), 400

    try:
        db.resolve_alert(int(alert_id))
        return jsonify({'success': True})
    except Exception as e:
        return jsonify({'success': False, 'message': str(e)}), 400


# ============================================================
# API: Threshold config (admin)
# ============================================================
@app.route('/api/thresholds')
def get_thresholds():
    # Cho phép user thường xem, admin chỉnh
    login_err = require_login()
    if login_err:
        return login_err
    return jsonify(db.get_all_thresholds())


@app.route('/api/thresholds/update', methods=['POST'])
def update_thresholds():
    admin_err = require_admin()
    if admin_err:
        return admin_err

    data = request.get_json(force=True) or {}
    config = data.get('config')  # dict {key: value}

    if not isinstance(config, dict) or not config:
        return jsonify({'success': False, 'message': 'config must be an object'}), 400

    try:
        for key, value in config.items():
            db.set_threshold(str(key), value, account_id=session['user_id'])
        return jsonify({'success': True})
    except Exception as e:
        return jsonify({'success': False, 'message': str(e)}), 400


# ============================================================
# API: Accounts (admin)
# ============================================================
@app.route('/api/accounts', methods=['GET'])
def list_accounts_api():
    admin_err = require_admin()
    if admin_err:
        return admin_err
    return jsonify(db.list_accounts())


@app.route('/api/accounts/add', methods=['POST'])
def add_account_api():
    admin_err = require_admin()
    if admin_err:
        return admin_err

    data = request.get_json(force=True) or {}
    username = (data.get('username') or '').strip()
    password = (data.get('password') or '').strip()
    role = (data.get('role') or 'viewer').strip()
    full_name = (data.get('full_name') or '').strip() or None

    if not username or not password:
        return jsonify({'success': False, 'message': 'Missing username/password'}), 400
    if role not in ('admin', 'user', 'viewer'):
        return jsonify({'success': False, 'message': 'Invalid role'}), 400

    try:
        new_id = db.add_account(username=username, plain_password=password, role=role, full_name=full_name)
        return jsonify({'success': True, 'id': new_id})
    except Exception as e:
        return jsonify({'success': False, 'message': str(e)}), 400


@app.route('/api/accounts/deactivate', methods=['POST'])
def deactivate_account_api():
    admin_err = require_admin()
    if admin_err:
        return admin_err

    data = request.get_json(force=True) or {}
    account_id = data.get('account_id')
    if not account_id:
        return jsonify({'success': False, 'message': 'Missing account_id'}), 400

    try:
        db.deactivate_account(int(account_id))
        return jsonify({'success': True})
    except Exception as e:
        return jsonify({'success': False, 'message': str(e)}), 400


@app.route('/api/accounts/change-password', methods=['POST'])
def change_password_api():
    admin_err = require_admin()
    if admin_err:
        return admin_err

    data = request.get_json(force=True) or {}
    account_id = data.get('account_id')
    new_password = (data.get('new_password') or '').strip()

    if not account_id or not new_password:
        return jsonify({'success': False, 'message': 'Missing account_id/new_password'}), 400

    try:
        db.change_password(int(account_id), new_password)
        return jsonify({'success': True})
    except Exception as e:
        return jsonify({'success': False, 'message': str(e)}), 400


# ============================================================
# API: History - Control commands / Door access (admin + user)
# ============================================================
@app.route('/api/logs/commands')
def get_control_commands_logs():
    login_err = require_login()
    if login_err:
        return login_err
    logs = db.get_recent_commands(limit=100)
    return jsonify(logs)


@app.route('/api/logs/door-access')
def get_door_access_logs():
    login_err = require_login()
    if login_err:
        return login_err

    result_filter = request.args.get('result')  # SUCCESS/FAILED/LOCKED
    logs = db.get_door_access_log(limit=100, result_filter=result_filter)
    return jsonify(logs)


# ============================================================
# SERIAL BRIDGE (thread nền trong Flask)
# ============================================================
def _start_serial_bridge_background():
    """
    DEPRECATED: Serial thread không chạy trong Flask nữa.
    Để tránh conflict với serial_reader.py (chạy riêng),
    Flask chỉ ghi PENDING commands vào DB.
    serial_reader.py sẽ đọc, gửi xuống COM, và xử lý response.
    """
    global _SERIAL_THREAD_STARTED
    _SERIAL_THREAD_STARTED = True
    print("[SERIAL-BRIDGE] Disabled - use serial_reader.py instead")
    return


# ============================================================
# Start
# ============================================================
if __name__ == '__main__':
    _start_serial_bridge_background()
    app.run(debug=True, host='0.0.0.0', port=5000)
