using MonoMod;

namespace Modding.Patches;

[MonoModPatch("global::HeroController")]
public class HeroController : global::HeroController
{
    private void orig_Start() { }
    private void orig_Update() { }

    private void Start()
    {
        ModHooks.OnHeroStart();
        orig_Start();
    }

    private void Update()
    {
        ModHooks.OnHeroUpdate();
        orig_Update();
    }
}