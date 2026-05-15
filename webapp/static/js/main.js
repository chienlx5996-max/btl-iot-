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
    // Chặn thao tác thủ công khi đang bật Auto
    const autoEl = document.getElementById('auto-switch');
    const isAutoOn = autoEl ? autoEl.checked : false;

    if (devicePrefix !== 'MODE' && isAutoOn) {
        // Nếu Auto đang bật mà user bấm điều khiển thủ công (quạt/cửa/cửa sổ/còi),
        // thì chuyển sang MANUAL trước để tránh Arduino autoLogic ghi đè khiến “không mở/tắt được”.
        try {
            await fetch('/api/control', {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({action: 'MODE_MANUAL'})
            });
            // Chờ 1 chút để Arduino/DB chuyển trạng thái
            await new Promise(resolve => setTimeout(resolve, 300));
            updateStatus();
        } catch (e) {
            console.error("[Control] Failed to switch MODE_MANUAL:", e);
            alert("Lỗi kết nối server khi chuyển sang Thủ công.");
            updateStatus();
            return;
        }
    }

    // Get the element and the NEW state (after toggle)
    // NOTE: IDs in index.html:
    //   FAN/DOOR/WINDOW/BUZZER use: <prefix>-switch (lowercase)
    //   MODE uses: auto-switch (not mode-switch)
    const elementId =
        devicePrefix === 'MODE'
            ? 'auto-switch'
            : devicePrefix.toLowerCase() + '-switch';

    const switchElement = document.getElementById(elementId);
    if (!switchElement) {
        console.error(`[Control] Missing element id='${elementId}' for devicePrefix='${devicePrefix}'`);
        return;
    }

    const isChecked = switchElement.checked; // Get current state after toggle
    let action = '';

    if (devicePrefix === 'MODE') {
        action = isChecked ? 'MODE_AUTO' : 'MODE_MANUAL';
    } else {
        action = devicePrefix + (isChecked ? '_ON' : '_OFF');
        if (devicePrefix === 'DOOR' || devicePrefix === 'WINDOW') {
            action = devicePrefix + (isChecked ? '_OPEN' : '_CLOSE');
        }
    }

    console.log(`[Control] Device: ${devicePrefix}, State: ${isChecked}, Action: ${action}`);

    try {
        const resp = await fetch('/api/control', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({action: action})
        });
        const result = await resp.json();
        console.log(`[Control] Response: ${JSON.stringify(result)}`);
        
        if (!result.success) {
            alert("Lỗi: " + result.message);
            updateStatus(); // Revert UI
        } else {
            // If switching to MANUAL mode (turning OFF auto), turn off all devices immediately
            if (devicePrefix === 'MODE' && !isChecked) {
                console.log("[Control] Triggering AUTO_SHUTDOWN");
                setTimeout(() => sendAutoShutdown(), 500);
            }
        }
    } catch (e) {
        alert("Lỗi kết nối server");
        console.error("[Control] Error:", e);
    }
}

// --- Auto Shutdown (turn off all devices) ---
async function sendAutoShutdown() {
    try {
        const resp = await fetch('/api/control', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({action: 'AUTO_SHUTDOWN'})
        });
        const result = await resp.json();
        console.log("Auto shutdown response:", result);
    } catch (e) {
        console.error("Auto shutdown error:", e);
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
