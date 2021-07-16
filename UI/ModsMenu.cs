using System;
using System.Collections;
using UnityEngine;

using NSEipix.View.UI;

using GM_Core.API.UI;

namespace GM_Core.UI
{
    internal class ModsMenu : MonoBehaviour
    {
        private GameObject _btnOpenMods;
        private GameObject _divOpenMods;

        private void Awake()
        {
            _btnOpenMods = MainMenuUI.CreateMenuButton("ModsButton", "Mods", 12, OnModsButtonClick);
            _divOpenMods = MainMenuUI.CreateDivider(13);
            
            GMCore.Logging.LogInfo("Mods menu loaded!");
        }

        private void OnModsButtonClick()
        {
            
        }
    }
}