using System;

using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

using NSMedieval.UI;

using GM_Core.API.UI;
using GM_Core.UI;
using GM_Core.Core;

namespace GM_Core
{
    [BepInPlugin("atilion.mods.GMCore", "Going Medieval Core", "1.0.0")]
    internal class GMCore : BaseUnityPlugin
    {
        internal static GMCore Instance { get; private set; }
        internal static ManualLogSource Logging { get; private set; }

        internal ModsMenu _menuMods;
        internal PluginLoader _pluginLoader;
        
        // Setup all of the easily accessible variables and load the PluginLoader game object to load game specific plugins
        // Then we also setup the events to trigger different parts of the mod based on different events
        void Awake()
        {
            Instance = this;
            Logging = Logger;

            _pluginLoader = gameObject.AddComponent<PluginLoader>();

            SceneUIManager.OnViewShown += OnUIViewShown;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // Called when the home screen/main menu is loaded as a scene
        // Then we setup the MainMenuUI's required variables and load the ModsMenu game object to create a mods menu in the main menu
        private void InvokeHomeScene()
        {
            // Due to this triggering multiple times, we should only assign it once the game object isn't active
            if (MainMenuUI.mainMenuInstance == null)
            {
                Logging.LogInfo("Populating mainMenuInstance field as it was null!");
                MainMenuUI.mainMenuInstance = GameObject.FindObjectOfType<MainMenuView>();
                if (MainMenuUI.mainMenuInstance == null) return;
            }

            // Prevent creation of multiple instances of the mods menu, as the event is called every time the menu is switched
            if (_menuMods == null)
                _menuMods = MainMenuUI.mainMenuInstance.gameObject.AddComponent<ModsMenu>();
        }

        // Used for detecting scene changes and loading of scenes to do different actions based on which scene is loaded
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Logger.LogInfo("Loaded Scene: " + scene.name);

            switch(scene.name)
            {
                case "HomeScene":
                    InvokeHomeScene();
                    break;
                default:
                    break;
            }
        }
        private void OnUIViewShown(string viewName)
        {
            Logger.LogInfo("Shown UI: " + viewName);
        }
    }
}