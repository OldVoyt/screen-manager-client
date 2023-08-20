using System;
using System.Diagnostics;

namespace ScreenManagerClient.Logic;

public enum LogLevel
{
    Debug,
    Info,
}
public class Logger
{
    private static NLog.Logger logger = NLog.LogManager.GetLogger("General");

    private readonly LogLevel _logLevel;
    private static Logger _instance = new Logger();
    
    public static Logger GetLogger()
    {
        return _instance;
    }

    private Logger(LogLevel logLevel = LogLevel.Debug)
    {
        _logLevel = logLevel;
    }

    public static void SetLogLevel(LogLevel logLevel)
    {
        _instance = new Logger(logLevel);
    }

    public void LogDebug(string message)
    {
        if (_logLevel != LogLevel.Debug) return;
        Debug.WriteLine("[INFO] " + message);
        logger.Debug(message);
    }
    public void LogInfo(string message)
    {
        Debug.WriteLine("[INFO] " + message);
        logger.Info(message);
    }

    public void LogError(string message, Exception e)
    {
        Debug.WriteLine("[ERROR] " + message + e.ToString());
        logger.Error(e);
    }
}
