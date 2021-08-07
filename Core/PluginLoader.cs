using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;

using UnityEngine;

namespace GM_Core.Core
{
    internal class PluginLoader : MonoBehaviour
    {
        private Harmony _harmonyInstance;

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

        private static void JSON_Deserialize(object __instance)
        {
            MethodInfo miJsonFile = __instance.GetType().GetMethod("JsonFile", BindingFlags.Instance | BindingFlags.NonPublic);
            if (miJsonFile == null) return;

            GMCore.Logging.LogInfo("Detected JSON load hit! " + (string)miJsonFile.Invoke(__instance, new object[0]));
        }

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
        }
    }
}
