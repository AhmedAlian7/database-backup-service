# 📦 Database Backup Windows Service

A Windows Service built with C# (.NET Framework 4.7.2) that automates SQL Server database backups on a configurable schedule. The service deletes old backups, creates new `.bak` files, and logs all operations for reliability and traceability.

---

## ✅ Features

- ⏰ Scheduled automatic database backups at configurable intervals
- 🗑️ Automatic deletion of old backup files before each new backup
- 💾 Backup files saved to a configurable folder with timestamped filenames
- 📝 Detailed logging of all backup and file operations with timestamps
- ⚙️ All settings (connection string, backup/log folders, interval) configurable via `App.config`
- 🛠️ Can run silently as a Windows Service or interactively in console mode for debugging

---

## 🏗️ How It Works

1. The service starts and reads configuration from `App.config`.
2. At each interval:
   - Deletes any existing `.bak` files in the backup folder.
   - Connects to the specified SQL Server database.
   - Creates a new backup file named with the current date and time.
   - Logs the outcome of each operation (success or failure).
3. All paths, connection strings, and intervals are fully configurable.

---

## 🔧 Installation

1. **Build the Service**
   - Open the project in Visual Studio.
   - Switch the build configuration to Release.
   - Build the solution to generate the executable file (`DatabaseBackupService.exe`).

2. **Open Developer Command Prompt**
   - Run Developer Command Prompt for Visual Studio as Administrator.

3. **Install the Service**
   - Navigate to the directory containing `DatabaseBackupService.exe`.
   - Run:
     ```
     InstallUtil.exe DatabaseBackupService.exe
     ```

4. **Configure the Service**
   - Edit `App.config` to set your SQL Server connection string, backup folder, log folder, and backup interval.

---

## 🙏 Thank You

Thanks for checking out this project!

If you have suggestions, questions, or feedback, feel free to open an issue or submit a pull request.

Collaboration and improvements are always welcome!
