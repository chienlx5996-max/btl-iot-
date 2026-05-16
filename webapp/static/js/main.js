// --- Real-time Status Update ---
async function updateStatus() {
    try {
        const resp = await fetch('/api/status');
        const data = await resp.json();

        if (data.sensor) {
            document.getElementById('temp-val').textContent = data.sensor.temperature.toFixed(1) + '°C';
            document.getElementById('gas-val').textContent = data.sensor.gas_value;
            document.getElementById('rain-val').textContent = data.sensor.rain_status ? 'Có Mưa' : 'Không';
            document.getElementById('pir-val').textContent = data.sensor.pir_status ? 'Phát hiện' : 'Trống';
            
            // Update statuses
            const gasBadge = document.getElementById('gas-status');
            gasBadge.textContent = data.sensor.system_level.replace('_', ' ');
            gasBadge.className = 'status-badge ' + (data.sensor.system_level === 'AN_TOAN' ? 'badge-safe' : 'badge-danger');
        }

        if (data.device) {
            document.getElementById('fan-switch').checked = data.device.fan_status;
            document.getElementById('door-switch').checked = data.device.door_status;
            document.getElementById('window-switch').checked = data.device.window_status;
            document.getElementById('buzzer-switch').checked = data.device.buzzer_status;
            document.getElementById('auto-switch').checked = data.device.auto_mode;
            
            document.getElementById('system-message').textContent = data.device.last_message || 'Hệ thống đang chạy...';
            
            const levelBadge = document.getElementById('system-level');
            levelBadge.textContent = data.device.system_level.replace('_', ' ');
            levelBadge.className = 'status-badge ' + (data.device.system_level === 'AN_TOAN' ? 'badge-safe' : 'badge-danger');
        }
    } catch (e) {
        console.error("Lỗi cập nhật dữ liệu:", e);
    }
}

// --- Device Control ---
async function control(devicePrefix) {
    const isChecked = document.getElementById(devicePrefix.toLowerCase() + '-switch').checked;
    let action = '';

    if (devicePrefix === 'MODE') {
        action = isChecked ? 'MODE_AUTO' : 'MODE_MANUAL';
    } else {
        action = devicePrefix + (isChecked ? '_ON' : '_OFF');
        if (devicePrefix === 'DOOR' || devicePrefix === 'WINDOW') {
            action = devicePrefix + (isChecked ? '_OPEN' : '_CLOSE');
        }
    }

    try {
        const resp = await fetch('/api/control', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({action: action})
        });
        const result = await resp.json();
        if (!result.success) {
            alert("Lỗi: " + result.message);
            updateStatus(); // Revert UI
        }
    } catch (e) {
        alert("Lỗi kết nối server");
    }
}

// --- Load Logs ---
async function loadLogs() {
    try {
        const resp = await fetch('/api/logs/sensors');
        const logs = await resp.json();
        const tbody = document.getElementById('sensor-logs-body');
        tbody.innerHTML = '';

        logs.forEach(log => {
            const row = `
                <tr>
                    <td>${log.recorded_at}</td>
                    <td>${log.gas_value}</td>
                    <td>${log.temperature}°C</td>
                    <td>${log.pir_status ? 'Có' : 'Không'}</td>
                    <td>${log.rain_status ? 'Mưa' : 'Tạnh'}</td>
                    <td><span class="status-badge ${log.system_level === 'AN_TOAN' ? 'badge-safe' : 'badge-danger'}">${log.system_level}</span></td>
                </tr>
            `;
            tbody.innerHTML += row;
        });
    } catch (e) {
        console.error("Lỗi tải log:", e);
    }
}

// Khởi chạy
setInterval(updateStatus, 1500); // Cập nhật mỗi 1.5s
updateStatus();
loadLogs();
