using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;

namespace Cheeto
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        private const string PLUGIN_GUID = "com.cucumba.cheetos.warmode";
        private const string PLUGIN_NAME = "warmode-cheeto";
        private const string PLUGIN_VERSION = "1.0.0";

        public override void Load()
        {
            var harmony = new Harmony(PLUGIN_GUID);
            harmony.PatchAll();

            Log.LogInfo("cucumba loaded");
            Cheeto.Run(this);
        }
    }
}
