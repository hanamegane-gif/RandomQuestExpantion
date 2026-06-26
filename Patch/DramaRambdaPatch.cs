using HarmonyLib;
using RandomQuestExpantion.Config;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace RandomQuestExpantion.Patch
{
    // クエスト同時受注最大数を変更する
    // なんだかそれっぽく見えるQuestManager.MaxRandomQuestはどこにも使われていないのでパッチ不要
    [HarmonyPatch]
    internal class DramaRambdaPatch
    {
        [HarmonyTargetMethods]
        internal static IEnumerable<MethodBase> TargetMethods()
        {
            return typeof(DramaCustomSequence).GetNestedType("<>c__DisplayClass14_0", BindingFlags.NonPublic)
                                              .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                              .Where(m => m.Name == "<Build>b__25" || m.Name == "<Build>b__27");
        }

        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> ChangeMaxQuestLimitPatch(IEnumerable<CodeInstruction> instructions)
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
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DramaRambdaPatch), nameof(GetQuestLimitSetting)))
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
