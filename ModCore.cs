using BepInEx;
using HarmonyLib;
using RandomQuestExpantion.Patch;
using System.Collections.Generic;
using System;
using System.Reflection.Emit;
using System.Reflection;

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
                ImportExcelPatch.execImportThings(Info);
            }
        }

        internal static void Log(object payload)
        {
            Instance.Logger.LogInfo(payload);
        }

        internal static void LogILInstructions(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                OpCode opCode = instruction.opcode;
                object operand = instruction.operand;
                Console.WriteLine($"OpCode: {opCode} Operand: {operand}");
                switch (opCode.Name)
                {
                    case "ldfld":
                        if (operand is FieldInfo fieldInfo)
                        {
                            Console.WriteLine($"  Field: {fieldInfo.DeclaringType.Name}::{fieldInfo.Name}");
                        }
                        break;
                    case "call":
                        if (operand is MethodInfo methodInfo)
                        {
                            Console.WriteLine($"  Method: {methodInfo.DeclaringType.Name}::{methodInfo.Name}");
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}