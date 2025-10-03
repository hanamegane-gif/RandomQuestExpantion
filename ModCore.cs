using BepInEx;
using HarmonyLib;
using RandomQuestExpantion.Patch;
using System.Dynamic;
using System.IO;

namespace RandomQuestExpantion
{
    public static class ModInfo
    {
        internal const string GUID = "byakko.mods.RQX";
        internal const string Name = "RandomQuestExpantion";
        internal const string Version = "0.23.209";
    }

    [BepInPlugin(ModInfo.GUID, ModInfo.Name, ModInfo.Version)]
    internal class RandomQuestExpantion : BaseUnityPlugin
    {
        internal static RandomQuestExpantion Instance { get; private set; }
        internal static PluginInfo DLLInfo { get; private set; }

        private void Awake()
        {
            Instance = this;
            DLLInfo = Info;
            var harmony = new Harmony(ModInfo.GUID);

            harmony.PatchAll();
        }

        public void OnStartCore()
        {
            if (DeployModMap.DeployModMaps(DLLInfo))
            {
                DeployTypeFallback.DeployTypeFallbackSetting();
                ImportExcelPatch.execImportQuests(Info);
                ImportExcelPatch.execImportLanguages(Info);
                ImportExcelPatch.execImportZones(Info);
            }
        }

        internal static void Log(object payload)
        {
            Instance.Logger.LogInfo(payload);
        }
    }
}