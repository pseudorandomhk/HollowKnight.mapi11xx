using MonoMod;

namespace Modding.Patches;

[MonoModPatch("global::PlayMakerUnity2DProxy")]
public class PlayMakerUnity2DProxy : global::PlayMakerUnity2DProxy
{
    private void orig_Start() { }

    private new void Start()
    {
        orig_Start();
        ModHooks.OnColliderCreate(gameObject);
    }
}