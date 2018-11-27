using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace DotNetCoreDashboardArch.Services
{
    public interface ICacheDependency : IDisposable
    {
        DateTime LastUpdated { get; set; }
    }

    public class CacheDependency : ICacheDependency
    {
        private string _connString;
        private SqlConnection _conn;
        private SqlDependency _dependency;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private string _cmdstring = "SELECT t.record FROM table t Where t.changed = true";
        private string _queue = "DataChangeMessages";
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public CacheDependency(string connString, ILoggerFactory loggerFactory)
        {
            _connString = connString;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger("SQLCacheDependency");
            Initialize();
            RegisterNotification();
        }

        private void Initialize()
        {
            try
            {
                SqlDependency.Start(_connString, _queue);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException)
                {
                    LogErrors(ex, "Attempting service queue cleanup");
                    string _sqlcmdstring = "CleanupQueue";
                    try
                    {
                        using (_conn = new SqlConnection(_connString))
                        {
                            _conn.Open();
                            SqlCommand cmd = new SqlCommand(_sqlcmdstring, _conn);
                            cmd.ExecuteNonQuery();
                        }
                        SqlDependency.Start(_connString, _queue);
                        LastUpdated = DateTime.Now;
                        _logger.LogInformation("Cleanup successful");
                    }
                    catch (Exception e2)
                    {
                        LogErrors(e2);
                        LastUpdated = DateTime.MinValue;
                    }
                }
                else
                {
                    LogErrors(ex);
                    LastUpdated = DateTime.MinValue;
                }
            }
        }

        private void RegisterNotification()
        {   
            try
            {
                using (_conn = new SqlConnection(_connString))
                {
                    _conn.Open();
                    SqlCommand cmd = new SqlCommand(_cmdstring, _conn);
                    _dependency = new SqlDependency(cmd, "Service=DataChangeNotifications", 0);
                    _dependency.OnChange += NotificationHandler;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                    }
                    reader.Close();
                }
            } catch (SqlException ex)
            {
                LogErrors(ex);
                LastUpdated = DateTime.MinValue;
            }
        }

        private void NotificationHandler(object sender, SqlNotificationEventArgs e)
        {
            _logger.LogInformation("Data change notification received. Updating timestamp and re-registering listener.");
            LastUpdated = DateTime.Now;
            RegisterNotification();
        }

        private void LogErrors(Exception ex, string customMsg = null)
        {
            int errorCode = ex.HResult;
            string errorType = ex.GetType().Name;
            string errorMsg = ex.Message;
            _logger.LogError(String.Format("{0}: {1} while initializing SQL Cache Dependency", errorCode, errorType));
            _logger.LogError(String.Format("{0}: {1}", errorCode, errorMsg));
            if (customMsg != null)
            {
                _logger.LogWarning(customMsg);
            }
        }

        public void Dispose()
        {
            SqlDependency.Stop(_connString, _queue);
        }
    }
}
