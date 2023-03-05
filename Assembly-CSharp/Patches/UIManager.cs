using MonoMod;
using UnityEngine;

namespace Modding.Patches;

[MonoModPatch("global::UIManager")]
public class UIManager : global::UIManager
{
    [MonoModIgnore] private static UIManager _instance;

    public static UIManager get_instance()
    {
        if (UIManager._instance == null)
        {
            UIManager._instance = FindObjectOfType<UIManager>();
            if (UIManager._instance != null && Application.isPlaying)
                DontDestroyOnLoad(UIManager._instance.gameObject);
        }

        return UIManager._instance;
    }
}