using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;

using UnityEngine;

namespace GM_Core.Core
{
    internal class PluginLoader : MonoBehaviour
    {
        private Harmony _harmonyInstance;

        private static Dictionary<string, string> _virtualToRealConfig = new Dictionary<string, string>(); // Bad life decisions up ahead

        private static MethodInfo _methodDeserializeJson = null;
        private static HarmonyMethod _harmonyDeserializeJson = null;

        void Awake()
        {
            _harmonyInstance = new Harmony("atilion.mods.GMCore.PluginLoader");

            _methodDeserializeJson = typeof(NSMedieval.Construction.GraveRepository).BaseType.GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.NonPublic);
            _harmonyDeserializeJson = new HarmonyMethod(typeof(PluginLoader).GetMethod("JSON_Deserialize", BindingFlags.Static | BindingFlags.NonPublic));

            _harmonyInstance.Patch(_methodDeserializeJson, postfix: _harmonyDeserializeJson);

            foreach(string pluginDir in Directory.GetDirectories(Paths.PluginPath))
            {
                GMCore.Logging.LogInfo("Loading plugin directory: " + pluginDir);

                // Load JSON mods
                string jsonPath = pluginDir + "\\JSON";
                if (Directory.Exists(jsonPath))
                    LoadJSONPlugin(jsonPath, "");
            }
        }

        // Called when the JsonRepository in the game finishes deserialization of the file
        // Then we apply our new JSON values to the old ones???
        private static void JSON_Deserialize(object __instance)
        {
            MethodInfo miJsonFile = __instance.GetType().GetMethod("JsonFile", BindingFlags.Instance | BindingFlags.NonPublic);
            if (miJsonFile == null) return;

            string virtualPath = (string)miJsonFile.Invoke(__instance, new object[0]);
            GMCore.Logging.LogInfo("Detected JSON load hit! " + virtualPath);
            if (!_virtualToRealConfig.ContainsKey(virtualPath)) return;

            string customJson = File.ReadAllText(_virtualToRealConfig[virtualPath]);
            if (string.IsNullOrEmpty(customJson)) return;

            try
            {
                object customObject = JsonUtility.FromJson(customJson, __instance.GetType().BaseType.GetGenericArguments()[1]);

                GMCore.Logging.LogInfo("Successfully loaded custom JSON object for " + virtualPath);
            }
            catch(Exception e) { GMCore.Logging.LogError(e); }
        }

        // Loads the JSON modifications of each plugin with a virtual and physical path
        // Once loaded it stores it into a dictionary _virtualToRealConfig for easier lookup during actual JSON deserialization
        private void LoadJSONPlugin(string path, string virtualPath)
        {
            // Handle directory loading
            FileAttributes attributes = File.GetAttributes(path);
            if(attributes.HasFlag(FileAttributes.Directory))
            {
                foreach(string file in Directory.GetFiles(path))
                    LoadJSONPlugin(file, virtualPath + Path.GetFileName(file));
                foreach(string dir in Directory.GetDirectories(path))
                    LoadJSONPlugin(dir, virtualPath + Path.GetFileName(dir) + "/");
                return;
            }

            // Handle file loading
            if (!virtualPath.EndsWith(".json")) return;
            GMCore.Logging.LogInfo("Found JSON plugin file: " + virtualPath);
            _virtualToRealConfig.Add(virtualPath, path);
        }
    }
}
