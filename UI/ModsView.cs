using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using TMPro;

using BepInEx;
using BepInEx.Bootstrap;

using NSMedieval.UI;
using NSEipix.View.UI;

namespace GM_Core.UI
{
    internal class ModsView : ClosableUIView
    {
        private GameObject _closeButtonObject;
        private SoundButton _closeButton;

        private FieldInfo _saveProfilePrefabField;
        private GameObject _saveProfilePrefab;

        private GameObject _profileList;

        private Dictionary<PluginInfo, GameObject> _pluginList = new Dictionary<PluginInfo, GameObject>();

        void Awake()
        {
            Transform deleteButton = transform.Find("Content/Footer/Delete");
            if(deleteButton == null)
            {
                GMCore.Logging.LogWarning("Failed to find the delete button for mods!");
                return;
            }
            _closeButtonObject = deleteButton.gameObject;

            Transform backButton = transform.Find("Content/Footer/Back");
            if(backButton != null) GameObject.Destroy(backButton.gameObject);

            Transform nextButton = transform.Find("Content/Footer/Next");
            if (nextButton != null) GameObject.Destroy(nextButton.gameObject);

            _saveProfilePrefabField = typeof(ProfilesView).GetField("saveProfilePrefab", BindingFlags.NonPublic | BindingFlags.Instance);
            if(_saveProfilePrefabField == null)
            {
                GMCore.Logging.LogWarning("Could not find saveProfilePrefab field in ProfilesView");
                GameObject.Destroy(gameObject);
                return;
            }

            ProfilesView profilesView = GMCore.Instance._menuMods._loadGameView.GetComponent<ProfilesView>();
            if (profilesView == null)
            {
                GMCore.Logging.LogWarning("Could not find profiles view!");
                GameObject.Destroy(gameObject);
                return;
            }
            _saveProfilePrefab = (GameObject)_saveProfilePrefabField.GetValue(profilesView);

            Transform profileList = transform.Find("Content/Body/Scroll View/Viewport/ProfileList");
            if (profileList == null)
            {
                GMCore.Logging.LogWarning("Failed to find profile list!");
                GameObject.Destroy(gameObject);
                return;
            }
            _profileList = profileList.gameObject;

            _closeButton = _closeButtonObject.GetComponent<SoundButton>();
            if(_closeButton == null)
            {
                GMCore.Logging.LogWarning("Failed to find SoundButton component in close button!");
                GameObject.Destroy(_closeButtonObject);
                return;
            }

            Transform closeButtonTextObject = _closeButtonObject.transform.Find("Text");
            if(closeButtonTextObject == null)
            {
                GMCore.Logging.LogWarning("Failed to find text transform in close button!");
                GameObject.Destroy(_closeButtonObject);
                return;
            }

            TextMeshProUGUI closeButtonText = closeButtonTextObject.GetComponent<TextMeshProUGUI>();
            if(closeButtonText == null)
            {
                GMCore.Logging.LogWarning("Failed to find text in close button!");
                GameObject.Destroy(_closeButtonObject);
                return;
            }

            GameObject.DestroyImmediate(closeButtonTextObject.GetComponent<I2.Loc.Localize>());

            _closeButtonObject.name = "Close";
            closeButtonText.text = "Close";
        }
        void Start()
        {
            _closeButton.onClick.AddListener(() =>
            {
                SceneUIManager.ShowPreviousView();
            });

            foreach(var pluginEntry in Chainloader.PluginInfos)
                CreateModEntry(pluginEntry.Value.Metadata.Name);
            foreach (string pluginName in GMCore.Instance._pluginLoader._pluginList)
                CreateModEntry(pluginName);
        }

        public override void Show()
        {
            base.Show();
        }

        private void CreateModEntry(string modName)
        {
            GameObject plugin = GameObject.Instantiate(_saveProfilePrefab, _profileList.transform);
            if (plugin == null)
            {
                GMCore.Logging.LogWarning("Failed to instantiate plugin prefab!");
                GameObject.Destroy(plugin);
                return;
            }

            SettlementView settlementView = plugin.GetComponent<SettlementView>();
            if (settlementView != null) GameObject.Destroy(settlementView);

            Transform saveFileEntry = plugin.transform.Find("FilesList/SaveFileEntryObj");
            if (saveFileEntry == null)
                GMCore.Logging.LogWarning("Failed to find SaveFileEntryObj");
            if (saveFileEntry != null)
                GameObject.Destroy(saveFileEntry.gameObject);

            Transform rightGroup = plugin.transform.Find("Header/RightGroup");
            if (rightGroup == null)
                GMCore.Logging.LogWarning("Failed to find RightGroup");
            if (rightGroup != null)
                GameObject.Destroy(rightGroup.gameObject);

            Transform profileNameObj = plugin.transform.Find("Header/LeftGroup/ProfileName");
            if (profileNameObj == null)
            {
                GMCore.Logging.LogWarning("Failed to find ProfileName object!");
                GameObject.Destroy(plugin);
                return;
            }

            TextMeshProUGUI profileName = profileNameObj.gameObject.GetComponent<TextMeshProUGUI>();
            if (profileName == null)
            {
                GMCore.Logging.LogWarning("Failed to find the profile name text component!");
                GameObject.Destroy(plugin);
                return;
            }

            profileName.text = modName;
            GMCore.Logging.LogInfo("Successfully added plugin entry for " + profileName.text);
        }
    }
}
