using EasyLoggingLibrary;

FileLogger logger = new FileLogger("E:\\");

logger.WriteLog("Hello World", LogLevel.INFO);
logger.WriteLog("This is a warning!", LogLevel.WARN);

try
{
    File.OpenRead("ddf:dd.ff");
} catch (Exception ex)
{
    logger.WriteLog(ex);
}

logger.CloseLog();

Console.WriteLine("Press any key to continue...");
Console.ReadKey();