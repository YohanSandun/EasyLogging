using System.Text;

namespace EasyLoggingLibrary
{
    /// <summary>
    /// Represents log entry level.
    /// </summary>
    public enum LogLevel
    {
        FATAL, ERROR, INFO, WARN, DEBUG, TRACE
    }

    /// <summary>
    /// You can write log entries to files using this class.
    /// </summary>
    public class FileLogger
    {
        // Where log files will get created
        private string _path;

        // Formatted using DateTime.Now.ToString();
        private string _fileNamePattern;
        private string _fileExtention;
        private string _timestampPattern;

        // Maximum entries per log file
        private int _maxEntries;
        private int _entries = 0;

        private string _currentLogFile;
        private FileStream _currentStream;
        private bool _logOpened = false;

        /// <summary>
        /// Initializes a new instance of <c>FileLogger</c> with given parameters.
        /// </summary>
        /// <param name="path">Path to save log files. This path should be pointing to a directory not to a file.</param>
        /// <param name="fileNamePattern">File name pattern for the log files. This will get formatted using DateTime.ToString().</param>
        /// <param name="fileExtention">File extention for the log files.</param>
        /// <param name="timestampPattern">Timestamp pattern for each log entry. This will get formatted using DateTime.ToString().</param>
        /// <param name="maxEntries">Maximum number of log entries per file. Once the limit reached, LogSwitch will take a place.</param>
        public FileLogger(string path, string fileNamePattern = "dd-MM-yyyy HH-mm-ss", string fileExtention = ".txt", string timestampPattern = "HH:mm:ss", int maxEntries = 1000)
        {
            _path = path.EndsWith('\\') ? path : path + '\\';
            _fileNamePattern = fileNamePattern;
            _fileExtention = fileExtention;
            _maxEntries = maxEntries;
            _timestampPattern = timestampPattern;
            openLog();
        }

        /// <summary>
        /// Returns the current path where log files are written to.
        /// </summary>
        public string Path { get { return _path; } }

        /// <summary>
        /// Returns file name pattern of log files.
        /// </summary>
        public string FileNamePattern { get { return _fileNamePattern; } }

        /// <summary>
        /// Write a log entry using a message and log level.
        /// </summary>
        /// <param name="message">Message to be written to the log file.</param>
        /// <param name="level">Level of the log entry.</param>
        public void WriteLog(string message, LogLevel level)
        {
            if (!_logOpened)
                openLog();

            if (_entries >= _maxEntries)
                LogSwitch();

            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString(_timestampPattern));
            sb.Append(" [ ");
            sb.Append(level.ToString());
            sb.Append(" ]\t");
            sb.Append(message);
            sb.Append('\n');

            _currentStream.Write(Encoding.UTF8.GetBytes(sb.ToString()));
            _entries++;
        }

        /// <summary>
        /// Write a log entry using an exception. This will write two log entries.
        /// [ERROR] will contain information about the exception while [TRACE] will contain stack trace of the problem.
        /// </summary>
        /// <param name="exception"></param>
        public void WriteLog(Exception exception)
        {
            if (!_logOpened)
                openLog();

            if (_entries >= _maxEntries)
                LogSwitch();

            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString(_timestampPattern));
            sb.Append(" [ ERROR ]\t");
            sb.Append("HResult: ");
            sb.Append(exception.HResult);
            sb.Append(", Source: ");
            sb.Append(exception.Source);
            sb.Append(", Method: ");
            sb.Append(exception.TargetSite);
            sb.Append(", Message: ");
            sb.Append(exception.Message);
            sb.Append('\n');

            sb.Append(DateTime.Now.ToString("HH:mm:ss"));
            sb.Append(" [ TRACE ]\n");
            sb.Append(exception.StackTrace);
            sb.Append('\n');

            _currentStream.Write(Encoding.UTF8.GetBytes(sb.ToString()));
            _entries++;
        }

        /// <summary>
        /// Open a new log file.
        /// </summary>
        private void openLog()
        {
            string previousFile = _currentLogFile;
            _currentLogFile = _path + DateTime.Now.ToString(_fileNamePattern + "-fffff") + _fileExtention;
            
            while (previousFile == _currentLogFile)
                _currentLogFile = _path + DateTime.Now.ToString(_fileNamePattern + "-fffff") + _fileExtention;

            _currentStream = File.OpenWrite(_currentLogFile);
            _logOpened = true;
        }

        /// <summary>
        /// Close the current log file.
        /// </summary>
        public void CloseLog()
        {
            _logOpened = false;
            _entries = 0;
            _currentStream.Close();
        }

        /// <summary>
        /// Switch the current log file. This will first close the current opened log file and the open a new one.
        /// </summary>
        public void LogSwitch()
        {
            CloseLog();
            openLog();
        }
    }
}