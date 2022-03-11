using UnityEngine;
using BepInEx.Logging;

namespace Cheeto
{
    class Cheeto : MonoBehaviour
    {
        private static string ApplicationInfo = "";
        private static ManualLogSource Logger = null;

        public static void Run(Plugin loader)
        {
            Logger = loader.Log;

            ApplicationInfo = string.Format("{0} {1} {2} {3}", Application.productName, Application.version, Application.unityVersion, Application.buildGUID);
            Logger.LogInfo(ApplicationInfo);

            Cheeto instance = loader.AddComponent<Cheeto>();
            DontDestroyOnLoad(instance.gameObject);
            instance.hideFlags |= HideFlags.HideAndDontSave;

            CodeStage.AntiCheat.Detectors.ObscuredCheatingDetector.StopDetection();
            CodeStage.AntiCheat.Detectors.ObscuredCheatingDetector.Dispose();
            CodeStage.AntiCheat.Detectors.SpeedHackDetector.StopDetection();
            CodeStage.AntiCheat.Detectors.SpeedHackDetector.Dispose();
            CodeStage.AntiCheat.Detectors.InjectionDetector.StopDetection();
            CodeStage.AntiCheat.Detectors.InjectionDetector.Dispose();
        }
    }
}
