using System;
using System.Linq;
using GlobalEnums;
using UnityEngine;

namespace Modding.ModLoader;

internal class LoadedModsList : MonoBehaviour
{
    private GUIStyle style;
    private string text;

    private void Start()
    {
        style = new GUIStyle(GUIStyle.none)
        {
            normal = { textColor = Color.white },
            alignment = TextAnchor.UpperLeft,
            padding = new RectOffset(5, 5, 5, 5)
        };
        
        UpdateText();
    }

    private void OnGUI()
    {
        if (UIManager.instance == null)
            return;

        if (UIManager.instance.uiState is UIState.MAIN_MENU_HOME or UIState.PAUSED)
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text, style);
    }

    public void UpdateText()
    {
        text = $"Modding API: {MapiSettings.Version}\n" 
               + String.Join("\n", ModLoader.loadedMods.Select(m => $"{m.Name}: {m.Version}").ToArray());
    }
}