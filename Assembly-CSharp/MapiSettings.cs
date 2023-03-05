using System;
using System.IO;
using Modding.Logging;
using UnityEngine;
using Logger = Modding.Logging.Logger;

namespace Modding;

[Serializable]
internal class MapiSettings
{
    public const string Version = "0.1.0.0";

    public static readonly string SettingsPath =
        Path.Combine(Application.persistentDataPath, "ModdingApi-1114.GlobalSettings.json");
    
    public static MapiSettings Instance { get; private set; }

    static MapiSettings()
    {
        if (!File.Exists(SettingsPath))
        {
            ResetSettings();
            return;
        }

        try
        {
            Instance = JsonUtility.FromJson<MapiSettings>(File.ReadAllText(SettingsPath));
            Logger.API.LogInfo("Successfully loaded API global settings");
        }
        catch (Exception e)
        {
            Logger.API.LogError($"Exception while loading API settings, resetting:\n{e}");
            ResetSettings();
        }
    }

    public LogLevel LogLevel;
    public bool LogTimestamps;

    private MapiSettings()
    {
        LogLevel = LogLevel.INFO;
        LogTimestamps = false;
    }

    private static void ResetSettings()
    {
        Instance = new MapiSettings();
        try
        {
            File.WriteAllText(SettingsPath, JsonUtility.ToJson(Instance, true));
        }
        catch (Exception e)
        {
            Logger.API.LogError($"Exception while writing global settings:\n{e}");
            return;
        }
        Logger.API.LogInfo("Reset API global settings");
    }
}