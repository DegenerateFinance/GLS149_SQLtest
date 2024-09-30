using ConsoleTest;

namespace LogLib;

public struct LogEntry
{
    public DateTimeOffset TimeStamp { get; private set; }
    public string _message { get; private set; }
    public LogEntry(DateTimeOffset timeStamp, string message)
    {
        this.TimeStamp = timeStamp;
        this._message = message;
    }
}
public class LogManager
{
    public string LogName { get; private set; }
    public string LogNameWithDate { get; private set; }
    public Queue<LogEntry> LogEntries { get; private set; }
    private bool _finalizeLogManager;
    public int DaysToKeepLogs { get; private set; } = 7;
    public string LogFullPath { get; private set; }
    public string LogFullPathWithFile { get; private set; }
    private readonly object _lock = new object();
    private readonly AutoResetEvent _logEvent = new AutoResetEvent(false);
    private readonly Thread _logThread;

    //constructor --- call example: logManager = new LogManager("test", "C:\\IDESAI\\SEUR\\LOG\\")
    public LogManager(string _logName, string _logFolderPath, int _daysToKeepLogs)
    {
        this.LogName = _logName;
        this.DaysToKeepLogs = _daysToKeepLogs;
        LogEntries = new Queue<LogEntry>();

        //create log name with date
        LogNameWithDate = DateTime.Now.ToString("yyyy-MM-dd") + "_" + LogName + ".txt";

        this.LogFullPath = _logFolderPath;
        this.LogFullPathWithFile = Path.Combine(_logFolderPath, LogNameWithDate);
        
        //create log thread
        _logThread = new Thread(this.LogThread);
        _logThread.Name = "LogThread + " + LogNameWithDate;
        _logThread.Start();
    }
    private void CreateLogFile(string _logFolderPath)
    {
        //create log folder if it doesnt exist
        if (!Directory.Exists(_logFolderPath))
        {
            Directory.CreateDirectory(_logFolderPath);
        }
        //create the file if it doesnt exist
        if (!File.Exists(LogFullPathWithFile))
        {
            File.Create(LogFullPathWithFile).Close();
        }
    }
    private void DeleteOldLogs(string logFolderPath)
    {
        // Get all files and subdirectories in the specified folder
        string[] files = Directory.GetFiles(logFolderPath, "*", SearchOption.AllDirectories);
        string[] directories = Directory.GetDirectories(logFolderPath, "*", SearchOption.AllDirectories);

        try
        {
            // Delete files older than N days
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.CreationTime < DateTime.Now.AddDays(-DaysToKeepLogs) && fi.Name.Contains(LogName))
                {
                    fi.Delete();
                    this.AddEntry("Deleted old log: " + fi.Name);
                }
            }
            // Delete empty directories older than N days
            foreach (string directory in directories)
            {
                DirectoryInfo di = new DirectoryInfo(directory);
                if (di.CreationTime < DateTime.Now.AddDays(-DaysToKeepLogs) && di.GetFileSystemInfos().Length == 0)
                {
                    di.Delete();
                    this.AddEntry("Deleted old directory: " + di.Name);
                }
            } 
        }
        catch (Exception ex)
        {
            // This only happens if the Directory is modified during the process
            this.AddEntry("Error deleting old logs: " + ex.Message);
        }
    }
    public void AddEntry(string _message)
    {
        lock (_lock)
        {
            LogEntries.Enqueue(new LogEntry(DateTime.Now, _message));
        }
        _logEvent.Set();
    }
    public void AddEntry(string _message, DateTime d)
    {
        lock (_lock)
        {
            LogEntries.Enqueue(new LogEntry(d, _message));
        }
        _logEvent.Set();
    }
    public void SaveLog()
    {
        using (StreamWriter sw = File.AppendText(LogFullPathWithFile))
        {
            List<LogEntry> logEntriesCopy = new List<LogEntry>();
            lock (_lock)
            {
                while (LogEntries.Count > 0)
                {
                    LogEntry logEntry = LogEntries.Dequeue();
                    logEntriesCopy.Add(logEntry);
                }
            }
            foreach (LogEntry logEntry in logEntriesCopy)
            {
                sw.WriteLine(logEntry.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + logEntry._message + "\n");
            }
        }
    }

    
    private void LogThread()
    {
        CreateLogFile(LogFullPath);
        string versionInfo = "Version: " + BuildInfo.GetEntryVersionInfo();
        this.AddEntry($" ------------------- Log Started. {versionInfo} ------------------- ");
        DeleteOldLogs(LogFullPath);
        while (!_finalizeLogManager)
        {
            _logEvent.WaitOne();
            this.SaveLog();
        }
    }
    public void FinalizeLogManager()
    {
        string versionInfo = "Version: " + BuildInfo.GetEntryVersionInfo();
        this.AddEntry($" ------------------- Log Finished. {versionInfo} ------------------- \n\n");
        this._finalizeLogManager = true;
        _logEvent.Set();
        _logThread?.Join();
    }
    
}