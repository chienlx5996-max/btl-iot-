using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using BCrypt.Net;

namespace Smarthome
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(string? connectionString = null)
        {
            _connectionString = connectionString ??
                "Server=localhost;Port=3306;Database=nha_thong_minh;Uid=root;Pwd=123456789@;CharSet=utf8mb4;";
        }

        private MySqlConnection OpenConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        private DataTable Query(string sql, params MySqlParameter[] parameters)
        {
            var table = new DataTable();
            using var conn = OpenConnection();
            using var cmd = new MySqlCommand(sql, conn);
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            using var adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(table);
            return table;
        }

        private int Execute(string sql, params MySqlParameter[] parameters)
        {
            using var conn = OpenConnection();
            using var cmd = new MySqlCommand(sql, conn);
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            return cmd.ExecuteNonQuery();
        }

        public DataTable GetSensorData(int limit = 100, DateTime? fromDt = null, DateTime? toDt = null)
        {
            var sql = "SELECT `id`, `recorded_at`, `gas_value`, `temperature`, `pir_status`, `rain_status`, `system_level` " +
                      "FROM `sensor_data`";
            var conditions = new List<string>();
            var parameters = new List<MySqlParameter>();

            if (fromDt.HasValue)
            {
                conditions.Add("`recorded_at` >= @fromDt");
                parameters.Add(new MySqlParameter("@fromDt", fromDt.Value));
            }

            if (toDt.HasValue)
            {
                conditions.Add("`recorded_at` <= @toDt");
                parameters.Add(new MySqlParameter("@toDt", toDt.Value));
            }

            if (conditions.Count > 0)
            {
                sql += " WHERE " + string.Join(" AND ", conditions);
            }

            sql += " ORDER BY `recorded_at` DESC LIMIT @limit";
            parameters.Add(new MySqlParameter("@limit", limit));
            return Query(sql, parameters.ToArray());
        }

        public DataRow? GetLatestSensor()
        {
            var table = Query("SELECT * FROM `sensor_data` ORDER BY `recorded_at` DESC LIMIT 1");
            return table.Rows.Count > 0 ? table.Rows[0] : null;
        }

        public DataTable GetAlertHistory(int limit = 100, string? alertType = null, string? level = null, DateTime? fromDt = null)
        {
            var sql = "SELECT `id`, `alert_type`, `level`, `message`, `gas_value`, `temperature`, `pir_status`, `rain_status`, `created_at`, `resolved_at` " +
                      "FROM `alert_history`";
            var conditions = new List<string>();
            var parameters = new List<MySqlParameter>();

            if (!string.IsNullOrWhiteSpace(alertType))
            {
                conditions.Add("`alert_type` = @alertType");
                parameters.Add(new MySqlParameter("@alertType", alertType));
            }

            if (!string.IsNullOrWhiteSpace(level))
            {
                conditions.Add("`level` = @level");
                parameters.Add(new MySqlParameter("@level", level));
            }

            if (fromDt.HasValue)
            {
                conditions.Add("`created_at` >= @fromDt");
                parameters.Add(new MySqlParameter("@fromDt", fromDt.Value));
            }

            if (conditions.Count > 0)
            {
                sql += " WHERE " + string.Join(" AND ", conditions);
            }

            sql += " ORDER BY `created_at` DESC LIMIT @limit";
            parameters.Add(new MySqlParameter("@limit", limit));
            return Query(sql, parameters.ToArray());
        }

        public void ResolveAlert(long alertId)
        {
            Execute("UPDATE `alert_history` SET `resolved_at` = NOW(3) WHERE `id` = @id", new MySqlParameter("@id", alertId));
        }

        public DataTable GetDoorAccessLog(int limit = 100, string? resultFilter = null)
        {
            var sql = "SELECT `id`, `input_source`, `role_matched`, `result`, `wrong_count`, `note`, `opened_at`, `closed_at`, `duration_sec` " +
                      "FROM `door_access_log`";
            var parameters = new List<MySqlParameter>();
            if (!string.IsNullOrWhiteSpace(resultFilter))
            {
                sql += " WHERE `result` = @resultFilter";
                parameters.Add(new MySqlParameter("@resultFilter", resultFilter));
            }
            sql += " ORDER BY `opened_at` DESC LIMIT @limit";
            parameters.Add(new MySqlParameter("@limit", limit));
            return Query(sql, parameters.ToArray());
        }

        public DataRow GetDeviceStatus()
        {
            var table = Query("SELECT * FROM `device_status` WHERE `id` = 1");
            return table.Rows.Count > 0 ? table.Rows[0] : null;
        }

        public void UpdateDeviceStatus(
            int? fanStatus = null,
            int? doorStatus = null,
            int? windowStatus = null,
            int? buzzerStatus = null,
            string? ledState = null,
            int? autoMode = null,
            string? systemLevel = null,
            string? lastMessage = null)
        {
            var updates = new List<string>();
            var parameters = new List<MySqlParameter>();

            void AddUpdate(string field, object value)
            {
                updates.Add($"`{field}` = @{field}");
                parameters.Add(new MySqlParameter($"@{field}", value));
            }

            if (fanStatus.HasValue) AddUpdate("fan_status", fanStatus.Value);
            if (doorStatus.HasValue) AddUpdate("door_status", doorStatus.Value);
            if (windowStatus.HasValue) AddUpdate("window_status", windowStatus.Value);
            if (buzzerStatus.HasValue) AddUpdate("buzzer_status", buzzerStatus.Value);
            if (!string.IsNullOrWhiteSpace(ledState)) AddUpdate("led_state", ledState);
            if (autoMode.HasValue) AddUpdate("auto_mode", autoMode.Value);
            if (!string.IsNullOrWhiteSpace(systemLevel)) AddUpdate("system_level", systemLevel);
            if (!string.IsNullOrWhiteSpace(lastMessage)) AddUpdate("last_message", lastMessage);

            if (updates.Count == 0)
            {
                return;
            }

            var sql = "UPDATE `device_status` SET " + string.Join(", ", updates) + " WHERE `id` = 1";
            Execute(sql, parameters.ToArray());
        }

        public int InsertCommand(string source, string command, int? accountId = null, string? parameters = null)
        {
            using var conn = OpenConnection();
            using var cmd = new MySqlCommand(
                "INSERT INTO `control_commands` (`source`, `account_id`, `command`, `parameters`) VALUES (@source, @account_id, @command, @parameters)",
                conn);

            cmd.Parameters.AddWithValue("@source", source);
            cmd.Parameters.AddWithValue("@account_id", accountId.HasValue ? (object)accountId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@command", command);
            cmd.Parameters.AddWithValue("@parameters", string.IsNullOrWhiteSpace(parameters) ? (object)DBNull.Value : parameters);
            cmd.ExecuteNonQuery();
            return Convert.ToInt32(cmd.LastInsertedId);
        }

        public DataTable GetRecentCommands(int limit = 50)
        {
            return Query(
                "SELECT c.`id`, c.`source`, c.`command`, c.`parameters`, c.`status`, c.`response`, c.`sent_at`, c.`responded_at`, c.`created_at`, a.`username`, a.`role` " +
                "FROM `control_commands` c " +
                "LEFT JOIN `accounts` a ON c.`account_id` = a.`id` " +
                "ORDER BY c.`created_at` DESC LIMIT @limit", new MySqlParameter("@limit", limit));
        }

        public string? GetThreshold(string key)
        {
            var table = Query("SELECT `config_value` FROM `threshold_config` WHERE `config_key` = @key LIMIT 1", new MySqlParameter("@key", key));
            return table.Rows.Count > 0 ? table.Rows[0].Field<string>("config_value") : null;
        }

        public Dictionary<string, string> GetThresholds(IEnumerable<string> keys)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in keys)
            {
                var value = GetThreshold(key);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    result[key] = value!;
                }
            }
            return result;
        }

        public void SetThreshold(string key, string value, string dataType = "STRING", int? updatedBy = null)
        {
            Execute(
                "INSERT INTO `threshold_config` (`config_key`, `config_value`, `data_type`, `updated_by`) VALUES (@key, @value, @dataType, @updatedBy) " +
                "ON DUPLICATE KEY UPDATE `config_value` = VALUES(`config_value`), `data_type` = VALUES(`data_type`), `updated_by` = VALUES(`updated_by`)",
                new MySqlParameter("@key", key),
                new MySqlParameter("@value", value),
                new MySqlParameter("@dataType", dataType),
                new MySqlParameter("@updatedBy", updatedBy.HasValue ? updatedBy.Value : (object)DBNull.Value)
            );
        }

        public DataTable GetAccounts()
        {
            return Query("SELECT `id`, `username`, `role`, `full_name`, `is_active`, `last_login`, `created_at` FROM `accounts` ORDER BY `id`");
        }

        public void AddAccount(string username, string password, string role)
        {
            var hashed = BCrypt.Net.BCrypt.HashPassword(password);
            Execute(
                "INSERT INTO `accounts` (`username`, `password_hash`, `role`) VALUES (@username, @passwordHash, @role)",
                new MySqlParameter("@username", username),
                new MySqlParameter("@passwordHash", hashed),
                new MySqlParameter("@role", role.ToLower())
            );
        }

        public void UpdateAccount(int accountId, string? password = null, string? role = null)
        {
            var updates = new List<string>();
            var parameters = new List<MySqlParameter>();

            if (!string.IsNullOrWhiteSpace(password))
            {
                updates.Add("`password_hash` = @passwordHash");
                parameters.Add(new MySqlParameter("@passwordHash", BCrypt.Net.BCrypt.HashPassword(password)));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                updates.Add("`role` = @role");
                parameters.Add(new MySqlParameter("@role", role.ToLower()));
            }

            if (updates.Count == 0)
            {
                return;
            }

            parameters.Add(new MySqlParameter("@accountId", accountId));
            var sql = "UPDATE `accounts` SET " + string.Join(", ", updates) + " WHERE `id` = @accountId";
            Execute(sql, parameters.ToArray());
        }

        public void DisableAccount(int accountId)
        {
            Execute("UPDATE `accounts` SET `is_active` = 0 WHERE `id` = @id", new MySqlParameter("@id", accountId));
        }
    }
}
