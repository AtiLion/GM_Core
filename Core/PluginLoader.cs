using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;

using UnityEngine;

using NSEipix;
using NSEipix.FileReader;

namespace GM_Core.Core
{
    internal class PluginLoader : MonoBehaviour
    {
        private Harmony _harmonyInstance;

        private static Dictionary<string, List<string>> _virtualToRealConfig = new Dictionary<string, List<string>>(); // Bad life decisions up ahead

        private static MethodInfo _methodReadFile = null;
        private static HarmonyMethod _harmonyReadFile = null;

        void Awake()
        {
            _harmonyInstance = new Harmony("atilion.mods.GMCore.PluginLoader");

            Type t = typeof(NSMedieval.Construction.GraveRepository).Module.GetType("NSEipix.ObjectMapper.JsonSerializer`1");
            if (t == null)
            {
                GMCore.Logging.LogWarning("Failed to find JsonSerializer class! Ignoring JSON mods...");
                return;
            }

            _methodReadFile = FileReaders.Get.GetType().GetMethod("ReadFile", BindingFlags.Public | BindingFlags.Instance);
            _harmonyReadFile = new HarmonyMethod(typeof(PluginLoader).GetMethod("FileReaders_ReadFile", BindingFlags.NonPublic | BindingFlags.Static));

            _harmonyInstance.Patch(_methodReadFile, postfix: _harmonyReadFile);

            foreach(string pluginDir in Directory.GetDirectories(Paths.PluginPath))
            {
                GMCore.Logging.LogInfo("Loading plugin directory: " + pluginDir);

                // Load JSON mods
                string jsonPath = pluginDir + "\\JSON";
                if (Directory.Exists(jsonPath))
                    LoadJSONPlugin(jsonPath, "");
            }
        }

        // The ReadFile gets called each time we try to read any text based file from the game's save directory or root directory
        // To inject our own json files we wait for the function to load the text, we load any of our existing JSONs and merge them
        // Then we simply convert the new merged JObject back to JSON and return it as text
        // WARNING: Some problems can occur when multiple JSON files are overriding one another, still have to fix it by ignoring original data
        private static void FileReaders_ReadFile(IFileReader __instance, string __result, ref string fileName)
        {
            if (!fileName.EndsWith(".json") || !_virtualToRealConfig.ContainsKey(fileName)) return;
            GMCore.Logging.LogInfo("Found modded JSON request for " + fileName);

            try
            {
                JObject originalJObject = JObject.Parse(__result);
                JObject newJObject = (JObject)originalJObject.DeepClone();

                foreach(string jsonPath in _virtualToRealConfig[fileName])
                {
                    try
                    {
                        JObject modifiedJObject = JObject.Parse(File.ReadAllText(jsonPath));

                        newJObject.Merge(modifiedJObject, new JsonMergeSettings
                        {
                            MergeArrayHandling = MergeArrayHandling.Merge,
                            MergeNullValueHandling = MergeNullValueHandling.Merge
                        });
                    }
                    catch(Exception ex) { GMCore.Logging.LogError(ex); }
                }

                __result = newJObject.ToString(Formatting.None);
            }
            catch(Exception ex) { GMCore.Logging.LogError(ex); }
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

            List<string> jsonPaths;
            if (_virtualToRealConfig.ContainsKey(virtualPath))
                jsonPaths = _virtualToRealConfig[virtualPath];
            else
            {
                jsonPaths = new List<string>();
                _virtualToRealConfig.Add(virtualPath, jsonPaths);
            }
            jsonPaths.Add(path);
        }
    }
}
