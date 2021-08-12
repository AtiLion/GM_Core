using System;
using System.Collections;
using System.Reflection;

using NSEipix.View.UI;
using NSMedieval.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace GM_Core.API.UI
{
    public static class MainMenuUI
    {
        internal static MainMenuView mainMenuInstance = null;

        /// <summary>
        /// A coroutine pause that you can call yield on to wait for the main menu to be loaded
        /// </summary>
        /// <returns>Coroutine that waits for the main menu to be loaded</returns>
        public static IEnumerator WaitForMainMenu()
        {
            while (mainMenuInstance == null) yield return null;
        }

        #region Creation of UI elements
        /// <summary>
        /// Creates a main menu button on the specified position, which triggers the specified action once clicked
        /// </summary>
        /// <param name="name">The name of the button to assign to it</param>
        /// <param name="text">The text within the button</param>
        /// <param name="position">The index based position of the button on the list of menu buttons</param>
        /// <param name="clickAction">Executed when the user clicks on the button</param>
        /// <returns>Returns the menu button game object</returns>
        public static GameObject CreateMenuButton(string name, string text, ushort position, Action clickAction)
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

            SoundButton soundButton = newButton.GetComponent<SoundButton>();
            if (soundButton == null)
            {
                GMCore.Logging.LogWarning("Failed to find SoundButton component on button!");
                GameObject.Destroy(newButton);
                return null;
            }
            
            newButton.name = name;
            textMesh.text = text;
            newButton.transform.SetSiblingIndex(position);
            soundButton.PointerClickEvent += clickAction;

            return newButton;
        }
        /// <summary>
        /// Creates a divider between the main menu items/buttons at the specified position
        /// </summary>
        /// <param name="position">The index based position where the divider should be inserted into</param>
        /// <returns>The gameobject of the divider that was created</returns>
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
        #endregion
    }
}