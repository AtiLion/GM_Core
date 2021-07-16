using System;

using BepInEx;
using BepInEx.Logging;
using UnityEngine;

using NSMedieval.UI;

using GM_Core.API.UI;
using GM_Core.UI;

namespace GM_Core
{
    [BepInPlugin("atilion.mods.GMCore", "Going Medieval Core", "1.0.0")]
    internal class GMCore : BaseUnityPlugin
    {
        internal static GMCore Instance { get; private set; }
        internal static ManualLogSource Logging { get; private set; }

        internal ModsMenu _menuMods = null;
        
        private void Awake()
        {
            Instance = this;
            Logging = Logger;

            SceneUIManager.OnViewShown += OnUIViewShown;
        }

        private void OnUIViewShown(string viewName)
        {
            if (viewName != "MainMenuView") return;

            // Due to this triggering multiple times, we should only assign it once the game object isn't active
            if (MainMenuUI.mainMenuInstance == null)
            {
                Logging.LogInfo("Populating mainMenuInstance field as it was null!");
                MainMenuUI.mainMenuInstance = GameObject.FindObjectOfType<MainMenuView>();
                if (MainMenuUI.mainMenuInstance == null) return;
            }

            // Prevent creation of multiple instances of the mods menu, as the event is called every time the menu is switched
            if(_menuMods == null)
                _menuMods = MainMenuUI.mainMenuInstance.gameObject.AddComponent<ModsMenu>();
        }
    }
}