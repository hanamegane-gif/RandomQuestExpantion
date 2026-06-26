using HarmonyLib;
using RandomQuestExpantion.Config;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RandomQuestExpantion.Patch
{
    [HarmonyPatch]
    internal class LayerQuestBoardPatch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(LayerQuestBoard), nameof(LayerQuestBoard.RefreshQuest))]
        internal static IEnumerable<CodeInstruction> HandleOnShippedEventPatch(IEnumerable<CodeInstruction> instructions)
        {
            var ci = new CodeMatcher(instructions)
                .MatchEndForward
                (
                    new CodeMatch(OpCodes.Ldc_I4_5)
                )
                .Advance(1)
                .InsertAndAdvance
                (
                    new CodeInstruction(OpCodes.Pop),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LayerQuestBoardPatch), nameof(GetQuestLimitSetting)))
                )
                .InstructionEnumeration();
            return ci;
        }

        internal static int GetQuestLimitSetting()
        {
            return ModConfig.MaxQuestLimit;
        }
    }
}
