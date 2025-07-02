using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace DatabaseBackupService
{
    public partial class DatabaseBackupService : ServiceBase
    {
        private string _connectionString;
        private string _backupFolder;
        private string _logFolder;
        private int _backupIntervalMinutes;

        private string _logFilePath;
        private Timer _backupTimer;
        public DatabaseBackupService()
        {
            InitializeComponent();

            // Initialize configuration settings
            _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            _backupFolder = ConfigurationManager.AppSettings["BackupFolder"];
            _logFolder = ConfigurationManager.AppSettings["LogFolder"];
            _backupIntervalMinutes = int.Parse(ConfigurationManager.AppSettings["BackupIntervalMinutes"]);

            if (string.IsNullOrEmpty(_connectionString) 
                || string.IsNullOrEmpty(_backupFolder) 
                || string.IsNullOrEmpty(_logFolder) 
                || _backupIntervalMinutes <= 0)
            {
                throw new ConfigurationErrorsException("Invalid configuration settings for DatabaseBackupService.");
            }
            if (!Directory.Exists(_backupFolder))
            {
                System.IO.Directory.CreateDirectory(_backupFolder);
            }
            if(!Directory.Exists(_logFolder))
            {
                Directory.CreateDirectory(_logFolder);
            }
            _logFilePath = Path.Combine(_logFolder, "DatabaseBackupService.txt");
        }

        protected override void OnStart(string[] args)
        {
            LogToFile("Service started.");
            _backupTimer = new Timer(TimeSpan.FromMinutes(_backupIntervalMinutes).TotalMilliseconds); // Convert minutes to milliseconds
            _backupTimer.Elapsed += OnBackupTimerElapsed;
            _backupTimer.AutoReset = true;
            _backupTimer.Enabled = true;
            _backupTimer.Start();

        }

        private void OnBackupTimerElapsed(object sender, ElapsedEventArgs e)
        {

            // Delete existing .bak files
            var existingFiles = Directory.GetFiles(_backupFolder, "*.bak");
            foreach (var file in existingFiles)
            {
                try
                {
                    File.Delete(file);
                    LogToFile($"Deleted old backup file: {file}");
                }
                catch (Exception ex)
                {
                    LogToFile($"Failed to delete {file}: {ex.Message}");
                }
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string backupFileName = Path.Combine(_backupFolder, $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak");
                    string backupQuery = $"BACKUP DATABASE [{connection.Database}] TO DISK = '{backupFileName}' WITH INIT, SKIP, NOREWIND, NOUNLOAD, STATS = 10;";
                    using (var command = new SqlCommand(backupQuery, connection))
                    {
                        command.ExecuteNonQuery();
                        LogToFile($"Backup successful: {backupFileName}");
                    }
                }
                catch (Exception ex)
                {
                    LogToFile($"Backup failed: {ex.Message}");
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        protected override void OnStop()
        {
            _backupTimer?.Stop();
            _backupTimer?.Dispose();
            LogToFile("Service stopped.");  
        }

        private void LogToFile(string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";
            File.AppendAllText(_logFilePath, logMessage);

            if (Environment.UserInteractive)
            {
                Console.WriteLine(logMessage); // Output to console in debug mode
            }
        }

        [Conditional("DEBUG")]
        public void StartOnConsole()
        {
            OnStart(null);
            Console.WriteLine("Press Enter to stop the service...");
            Console.ReadLine();
            OnStop();

        }
    }
}
