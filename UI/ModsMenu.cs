using System;
using System.Linq;
using System.Collections;
using UnityEngine;

using NSEipix.View.UI;
using TMPro;

using GM_Core.API.UI;

namespace GM_Core.UI
{
    internal class ModsMenu : MonoBehaviour
    {
        private GameObject _btnOpenMods;
        private GameObject _divOpenMods;

        private GameObject _modsMenuView;

        private void Awake()
        {
            _btnOpenMods = MainMenuUI.CreateMenuButton("ModsButton", "Mods", 12, OnModsButtonClick);
            _divOpenMods = MainMenuUI.CreateDivider(13);
            
            GMCore.Logging.LogInfo("Mods menu loaded!");
        }

        private IEnumerator Start()
        {
            yield return MainMenuUI.WaitForMainMenu();

            if (!CreateModsMenu())
            {
                GameObject.Destroy(_btnOpenMods);
                GameObject.Destroy(_divOpenMods);

                if (_modsMenuView != null) GameObject.Destroy(_modsMenuView);
            }
        }

        private bool CreateModsMenu()
        {
            GameObject[] gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            GameObject loadGameView = gameObjects.FirstOrDefault(obj => obj.name == "LoadGameView");
            if(loadGameView == null)
            {
                GMCore.Logging.LogWarning("Failed to find LoadGameView!");
                return false;
            }

            _modsMenuView = GameObject.Instantiate(loadGameView, loadGameView.transform.parent);
            if(_modsMenuView == null)
            {
                GMCore.Logging.LogWarning("Failed to duplicate LoadGameView!");
                return false;
            }

            Transform titleObject = _modsMenuView.transform.Find("Content/VignetterHeader/Background/Title");
            if(titleObject == null)
            {
                GMCore.Logging.LogWarning("Failed to find the Title game object in mods menu!");
                return false;
            }

            TextMeshProUGUI titleText = titleObject.gameObject.GetComponent<TextMeshProUGUI>();
            if(titleText == null)
            {
                GMCore.Logging.LogWarning("Failed to find TextMeshProUGUI object in title object!");
                return false;
            }
            GMCore.Logging.LogInfo("Title game object name: " + titleObject.name);
            GMCore.Logging.LogInfo("Current title: " + titleText.text);

            titleText.text = "Mods";
            _modsMenuView.name = "ModsView";
            return true;
        }

        private void OnModsButtonClick()
        {
            _modsMenuView.SetActive(!_modsMenuView.activeSelf);
        }
    }
}