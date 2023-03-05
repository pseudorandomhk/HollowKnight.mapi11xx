using System.Collections;
using MonoMod;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable EventNeverSubscribedTo.Global
#pragma warning disable CS0414

namespace Modding.Patches;

[MonoModPatch("global::GameManager")]
public class GameManager : global::GameManager
{
    [MonoModIgnore] private bool tilemapDirty;
    [MonoModIgnore] private bool waitForManualLevelStart;
    [MonoModIgnore] public new event DestroyPooledObjects DestroyPersonalPools;
    [MonoModIgnore] public new event UnloadLevel UnloadingLevel;
    [MonoModIgnore] public new event LevelReady NextLevelReady;
    [MonoModIgnore] public new Scene nextScene { get; private set; }

    [MonoModIgnore]
    private extern void ManualLevelStart();
    
    [MonoModReplace]
    public new IEnumerator LoadSceneAdditive(string destScene)
    {
        tilemapDirty = true;
        startedOnThisScene = false;
        nextSceneName = destScene;
        waitForManualLevelStart = true;
        DestroyPersonalPools?.Invoke();
        UnloadingLevel?.Invoke();
        string exitingScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        nextScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(destScene);

        // Injected
        foreach (object o in ModHooks.OnBeforeAdditiveLoad(destScene))
            yield return o;
        
        AsyncOperation loadop = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(destScene, LoadSceneMode.Additive);
        loadop.allowSceneActivation = true;
        yield return loadop;
        UnityEngine.SceneManagement.SceneManager.UnloadScene(exitingScene);
        RefreshTilemapInfo(destScene);
        ManualLevelStart();
        NextLevelReady?.Invoke();
    }
}