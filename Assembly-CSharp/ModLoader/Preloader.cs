using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Modding.ModLoader;

internal static class Preloader
{
    public static IEnumerator Preload(Slider progressBar)
    {
        //no preloading yet
        
        progressBar.gameObject.SetActive(true);
        progressBar.minValue = 0;
        progressBar.maxValue = 1;
        
        AsyncOperation loadMenuOp = USceneManager.LoadSceneAsync(Constants.MENU_SCENE);
        while (!loadMenuOp.isDone)
        {
            progressBar.value = loadMenuOp.progress;
            yield return null;
        }
    }
}