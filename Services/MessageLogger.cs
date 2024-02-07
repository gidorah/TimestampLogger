using System;
using System.IO;

public class MessageLogger
{
    private readonly string _logFilePath;

    public MessageLogger(string logFilePath)
    {
        _logFilePath = logFilePath;
    }

    public void LogMessage(string message)
    {
        File.AppendAllText(_logFilePath, $"{DateTime.UtcNow}: {message}\n");
    }
}
