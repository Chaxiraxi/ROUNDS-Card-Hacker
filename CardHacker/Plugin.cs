using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using ROUNDSCheat.GUI;

namespace ROUNDSCheat
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("ROUNDS.exe")]
    public class ROUNDSCheatPlugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        public static ROUNDSCheatPlugin Instance { get; private set; }
        private GameObject guiObject;

        private void Awake()
        {
            Instance = this;
            Logger = base.Logger;

            // Apply Harmony patches
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.PLUGIN_GUID);

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} version {PluginInfo.PLUGIN_VERSION} is loaded!");
        }

        private void Start()
        {
            // Create GUI object
            guiObject = new GameObject("ROUNDSCheatGUI");
            guiObject.AddComponent<CardSelectionGUI>();
            DontDestroyOnLoad(guiObject);

            Logger.LogInfo("Card Selection GUI initialized. Press F1 to open/close.");
        }

        private void OnDestroy()
        {
            if (guiObject != null)
            {
                Destroy(guiObject);
            }
        }
    }
}
