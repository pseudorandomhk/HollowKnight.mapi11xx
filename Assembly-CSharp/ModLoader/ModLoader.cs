using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Modding.Utils;
using UnityEngine;
using Logger = Modding.Logging.Logger;

namespace Modding.ModLoader;

internal static class ModLoader
{
    public enum LoadState
    {
        NotStarted,
        Started,
        Loaded,
        Initialized
    }

    public static LoadState LoaderState { get; private set; } = LoadState.NotStarted;

    internal static readonly HashSet<Mod> loadedMods = new();

    public static void LoadMods()
    {
        if (LoaderState != LoadState.NotStarted)
        {
            Logger.API.LogWarn("LoadMods called multiple times, aborting");
            return;
        }
        
        LoaderState = LoadState.Started;
        Logger.API.LogInfo("Loading mods");

        string modDir = Application.platform switch
        {
            RuntimePlatform.WindowsPlayer or RuntimePlatform.LinuxPlayer =>
                Helper.CombinePaths(Application.dataPath, "Managed", "Mods"),
            RuntimePlatform.OSXPlayer =>
                Helper.CombinePaths(Application.dataPath, "Resources", "Data", "Managed", "Mods"),
            _ => null
        };

        if (modDir == null)
        {
            LoaderState = LoadState.Loaded;
            Logger.API.LogError($"Unsupported platform {Application.platform}, aborting");
            return;
        }

        if (!Directory.Exists(modDir))
        {
            Logger.API.LogInfo($"Mods directory did not exist, creating one at {modDir}");
            Directory.CreateDirectory(modDir);
            LoaderState = LoadState.Loaded;
            return;
        }

        foreach (Type t in LoadModTypesFromAssemblies(Directory.GetFiles(modDir, "*.dll")))
        {
            Mod mod;
            try
            {
                Logger.API.LogDebug($"Loading mod from type {t.Namespace}::{t.Name}");
                mod = t.GetConstructor(Type.EmptyTypes)?.Invoke(new object[0]) as Mod;
            }
            catch (Exception e)
            {
                Logger.API.LogWarn($"Failed to load mod {t.Name}:\n{e}\nContinuing...");
                continue;
            }

            if (mod == null)
            {
                Logger.API.LogWarn($"Failed to load mod {t.FullName} due to no parameterless constructor; continuing");
                continue;
            }

            Logger.API.LogInfo($"Successfully loaded mod {mod.Name}");
            loadedMods.Add(mod);
        }

        LoaderState = LoadState.Loaded;
        Logger.API.LogInfo($"Finished loading {loadedMods.Count} mod(s)");
        
        Logger.API.LogInfo("Initializing mod(s)");
        foreach (Mod m in loadedMods)
        {
            try
            {
                Logger.API.LogDebug($"Initializing mod {m.Name}");
                m.Initialize();
                Logger.API.LogInfo($"Successfully initialized mod {m.Name}");
            }
            catch (Exception e)
            {
                Logger.API.LogWarn($"Error initializing mod {m.Name}:\n{e}\nContinuing...");
            }
        }
        LoaderState = LoadState.Initialized;
        Logger.API.LogInfo("Finished initializing mod(s)");
    }

    private static List<Type> LoadModTypesFromAssemblies(string[] paths)
    {
        Logger.API.LogDebug($"Loading mods from {paths.Length} assemblies");
        List<Type> res = new();
        
        foreach (string path in paths)
        {
            Assembly asm;
            try
            {
                Logger.API.LogDebug($"Loading assembly {path}");
                asm = Assembly.LoadFrom(path);
            }
            catch (Exception e)
            {
                Logger.API.LogWarn($"Unable to load assembly from {path}:\n{e}\nContinuing...");
                continue;
            }

            Type[] types;
            try
            {
                Logger.API.LogDebug($"Loading types from {asm.FullName}");
                types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                Logger.API.LogWarn(
                    $"Encountered exception(s) while loading {path}:\n{String.Join("\n", e.LoaderExceptions.Select(le => le.Message).ToArray())}\nAttempting partial load...");
                types = e.Types.Where(t => t != null).ToArray();
            }
            catch (Exception e)
            {
                Logger.API.LogWarn($"Failed loading types from {path}:\n{e}\nContinuing...");
                continue;
            }
            
            res.AddRange(types.Where(t => !t.IsGenericType && t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Mod))));
        }

        return res;
    }
}