using MonoMod;
using UnityEngine;

namespace Modding.ModLoader;

[MonoModPatch("global::StartManager")]
internal class ModLoaderRunner : global::StartManager
{
    [MonoModReplace]
    private void Start()
    {
        ModLoader.LoadMods();
        StartCoroutine(Preloader.Preload(this.progressIndicator));
        
        DontDestroyOnLoad(new GameObject("Loaded Mod List").AddComponent<LoadedModsList>());
    }
}