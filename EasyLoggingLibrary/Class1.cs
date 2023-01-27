using System.Text;

namespace EasyLoggingLibrary
{
    public enum LogLevel
    {
        FATAL, ERROR, INFO, WARN, DEBUG, TRACE
    }

    public class FileLogger
    {
        // Where log files will get created
        private string _path;

        // Formatted using DateTime.Now.ToString();
        private string _fileNamePattern;
        private string _fileExtention;
        // Maximum entries per log file
        private int _maxEntries;
        private int _entries = 0;

        private string _currentLogFile;
        private FileStream _currentStream;
        private bool _logOpened = false;

        public FileLogger(string path, string fileNamePattern = "dd-MM-yyyy HH-mm-ss", string fileExtention = ".txt", int maxEntries = 1000)
        {
            _path = path.EndsWith('\\') ? path : path + '\\';
            _fileNamePattern = fileNamePattern;
            _fileExtention = fileExtention;
            _maxEntries = maxEntries;
            openLog();
        }

        public string Path { get { return _path; } }
        public string FileNamePattern { get { return _fileNamePattern; } }

        public void WriteLog(string message, LogLevel level)
        {
            if (!_logOpened)
                openLog();

            if (_entries >= _maxEntries)
                LogSwitch();

            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("HH:mm:ss"));
            sb.Append(" [ ");
            sb.Append(level.ToString());
            sb.Append(" ]\t");
            sb.Append(message);
            sb.Append('\n');

            _currentStream.Write(Encoding.UTF8.GetBytes(sb.ToString()));
            _entries++;
        }

        public void WriteLog(Exception exception)
        {
            if (!_logOpened)
                openLog();

            if (_entries >= _maxEntries)
                LogSwitch();

            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("HH:mm:ss"));
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

        private void openLog()
        {
            string previousFile = _currentLogFile;
            _currentLogFile = _path + DateTime.Now.ToString(_fileNamePattern + "-fffff") + _fileExtention;
            
            while (previousFile == _currentLogFile)
                _currentLogFile = _path + DateTime.Now.ToString(_fileNamePattern + "-fffff") + _fileExtention;

            _currentStream = File.OpenWrite(_currentLogFile);
            _logOpened = true;
        }

        public void CloseLog()
        {
            _logOpened = false;
            _entries = 0;
            _currentStream.Close();
        }

        public void LogSwitch()
        {
            CloseLog();
            openLog();
        }
    }
}