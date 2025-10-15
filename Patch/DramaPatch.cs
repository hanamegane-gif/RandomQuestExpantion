using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace RandomQuestExpantion.Patch
{
    // QuestTaskに独自のイベントハンドラを実装する
    [HarmonyPatch]
    class DramaPatch
    {
        [HarmonyTargetMethods]
        internal static IEnumerable<MethodBase> TargetMethods()
        {
            // 店投資と拠点投資共にメソッド名・引数が完全に一致していたので店投資のみのイベントハンドラを実装するのは諦めた
            // 流石にわざわざILまで見る処理を書く気にはなれない
            return typeof(DramaCustomSequence).GetNestedTypes(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                .Where(method => method.Name.Contains("__Invest"))
                .Cast<MethodBase>();
        }

        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> HandleOnInvestShopEventPatch(IEnumerable<CodeInstruction> instructions)
        {
            var ci = new CodeMatcher(instructions)
                .MatchStartForward
                (
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Zone), nameof(Zone.ModInfluence)))
                    //IL_006d: call class Zone EClass::get__zone()
                    //IL_0072: ldc.i4.1
	                //IL_0073: callvirt instance void Zone::ModInfluence(int32)

                    //IL_0078: call class Chara EClass::get_pc()
                )
                .Advance(1)
                .InsertAndAdvance
                (
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DramaPatch), nameof(HandleOnInvestShopEvent)))
                )
                .InstructionEnumeration();
            return ci;
        }

        internal static void HandleOnInvestShopEvent()
        {
            EClass.game.quests.list.ForeachReverse(delegate (Quest q)
            {
                q.OnInvestMod();
            });
        }
    }
}
