using System.Reflection;

using NSEipix.View.UI;
using NSMedieval.UI;
using UnityEngine;
using TMPro;

namespace GM_Core.API.UI
{
    public static class MainMenuUI
    {
        internal static MainMenuView mainMenuInstance = null;

        public static GameObject CreateMenuButton(string name, string text, ushort position)
        {
            if (mainMenuInstance == null)
            {
                GMCore.Logging.LogWarning("Menu instance not found! Ignoring call...");
                return null;
            }

            GameObject optionsButton = GameObject.Find("OptionsButton");
            if (optionsButton == null)
            {
                GMCore.Logging.LogWarning("Could not find options button!");
                return null;
            }

            GameObject newButton = GameObject.Instantiate(optionsButton, optionsButton.transform.parent);
            if (newButton == null)
            {
                GMCore.Logging.LogWarning("Could not create instance of options button!");
                return null;
            }

            GameObject textObject = newButton.transform.GetChild(0).gameObject;
            if (textObject == null)
            {
                GMCore.Logging.LogWarning("Failed to find text object in button!");
                GameObject.Destroy(newButton);
                return null;
            }

            TextMeshProUGUI textMesh = textObject.GetComponent<TextMeshProUGUI>();
            if (textMesh == null)
            {
                GMCore.Logging.LogWarning("Failed to find TextMeshProUGUI component on button!");
                GameObject.Destroy(newButton);
                return null;
            }
            
            newButton.name = name;
            textMesh.text = text;
            newButton.transform.SetSiblingIndex(position);

            return newButton;
        }

        public static GameObject CreateDivider(ushort position)
        {
            if (mainMenuInstance == null)
            {
                GMCore.Logging.LogWarning("Menu instance not found! Ignoring call...");
                return null;
            }
            
            GameObject divider = GameObject.Find("Divider_5");
            if (divider == null)
            {
                GMCore.Logging.LogWarning("Could not find Divider_5 game object!");
                return null;
            }

            GameObject newDivider = GameObject.Instantiate(divider, divider.transform.parent);
            if (newDivider == null)
            {
                GMCore.Logging.LogWarning("Could not create instance of divider game object!");
                return null;
            }

            newDivider.name = "Divider";
            newDivider.transform.SetSiblingIndex(position);

            return newDivider;
        }
    }
}