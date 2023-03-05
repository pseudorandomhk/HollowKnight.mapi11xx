using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Modding.Logging;

public class Logger
{
    private static readonly object logLock = new();
    private static readonly StreamWriter writer;

    private static readonly LogLevel logThreshold;
    private static readonly bool useTimestamp;

    internal static readonly Logger API = new("API");

    static Logger()
    {
        FileStream fs = new(Path.Combine(Application.persistentDataPath, "ModLog1114.txt"),
            FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        writer = new StreamWriter(fs, Encoding.UTF8) { AutoFlush = true };

        logThreshold = MapiSettings.Instance.LogLevel;
        useTimestamp = MapiSettings.Instance.LogTimestamps;
    }

    public readonly string Name;
    
    public Logger(string name)
    {
        Name = name ?? throw new ArgumentNullException();
    }

    public void Log(object msg) => LogInfo(msg);

    public void LogFine(object msg) => LogMessage(msg, LogLevel.FINE);
    public void LogDebug(object msg) => LogMessage(msg, LogLevel.DEBUG);
    public void LogInfo(object msg) => LogMessage(msg, LogLevel.INFO);
    public void LogWarn(object msg) => LogMessage(msg, LogLevel.WARN);
    public void LogError(object msg) => LogMessage(msg, LogLevel.ERROR);

    private void LogMessage(object msg, LogLevel level)
    {
        if (msg == null)
            throw new ArgumentNullException();
        if (level < logThreshold)
            return;

        string time = $"[{DateTime.Now:HH:mm:ss}] ";
        string prefix = $"{(useTimestamp ? time : "")}[{level}] [{Name}]: ";

        var lines = msg.ToString().Split('\n');
        lines[0] = prefix + lines[0];

        if (lines.Length > 1)
        {
            string pad = new string(' ', prefix.Length);
            for (int i = 1; i < lines.Length; i++)
                lines[i] = pad + lines[i];
        }

        lock (logLock)
        {
            writer.WriteLine(String.Join(Environment.NewLine, lines));
        }
    }
}