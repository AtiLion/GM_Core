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
        
        private void Awake()
        {
            Instance = this;
            Logging = Logger;

            SceneUIManager.OnViewShown += OnUIViewShown;
        }

        private void OnUIViewShown(string viewName)
        {
            if (viewName != "MainMenuView") return;
            Logging.LogInfo("Main menu UI has been shown!");
            
            MainMenuUI.mainMenuInstance = GameObject.FindObjectOfType<MainMenuView>();
            if (MainMenuUI.mainMenuInstance == null) return;

            MainMenuUI.mainMenuInstance.gameObject.AddComponent<ModsMenu>();
        }
    }
}