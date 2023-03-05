using System;
using System.Collections;
using UnityEngine;
using Logger = Modding.Logging.Logger;

// ReSharper disable PossibleInvalidCastExceptionInForeachLoop

namespace Modding;

public static class ModHooks
{
    #region GameManager_BeforeAdditiveLoad
    
    public delegate IEnumerable BeforeAdditiveLoadHandler(string destScene);
    public static event BeforeAdditiveLoadHandler BeforeAdditiveLoad;
    internal static IEnumerable OnBeforeAdditiveLoad(string destScene)
    {
        if (BeforeAdditiveLoad == null)
            yield break;

        foreach (BeforeAdditiveLoadHandler hook in BeforeAdditiveLoad.GetInvocationList())
        {
            IEnumerable iter;
            try
            {
                iter = hook(destScene);
            }
            catch (Exception e)
            {
                Logger.API.LogError($"Exception while retrieving BeforeAdditiveLoad handler from {hook.Method.DeclaringType?.Name}:\n{e}");
                continue;
            }

            foreach (object o in iter)
                yield return o;
        }
    }
    
    #endregion

    #region HeroController_Start

    public static event Action HeroStart;

    internal static void OnHeroStart() => HeroStart?.Invoke();

    #endregion

    #region HeroController_Update

    public static event Action HeroUpdate;
    internal static void OnHeroUpdate() => HeroUpdate?.Invoke();

    #endregion

    #region PlayerData_TakeHealth

    public delegate int TakeHealthProxy(int amount);
    public static event TakeHealthProxy TakeHealth;

    internal static int OnTakeHealth(int amount)
    {
        if (TakeHealth == null)
            return amount;

        int newAmount = amount;
        foreach (TakeHealthProxy hook in TakeHealth.GetInvocationList())
        {
            try
            {
                newAmount = hook(amount);
            }
            catch (Exception e)
            {
                Logger.API.LogError($"Exception while evaluating TakeHealth hook from {hook.Method.DeclaringType?.Name}:\n{e}");
            }
        }

        return newAmount;
    }

    #endregion

    #region PlayMakerUnity2DProxy_Start

    public static event Action<GameObject> ColliderCreate;

    internal static void OnColliderCreate(GameObject go) => ColliderCreate?.Invoke(go);

    #endregion
}