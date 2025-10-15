using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RandomQuestExpantion.Patch
{
    // QuestTaskに独自のイベントハンドラを実装する
    [HarmonyPatch]
    class GameDatePatch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(GameDate), nameof(GameDate.ShipGoods))]
        internal static IEnumerable<CodeInstruction> HandleOnShippedEventPatch(IEnumerable<CodeInstruction> instructions)
        {
            var ci = new CodeMatcher(instructions)
                .MatchStartForward
                (
                    new CodeMatch(OpCodes.Ldc_I4_1),
                    new CodeMatch(OpCodes.Add),
                    new CodeMatch(OpCodes.Stloc_2)
                    //IL_01d7: ldc.i4.1
                    //IL_01d8: add
                    //IL_01d9: stloc.2

                    //IL_01da: ldloc.s 7
                )
                .Advance(1)
                .InsertAndAdvance
                (
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameDatePatch), nameof(HandleOnShippedEvent)))
                )
                .InstructionEnumeration();
            return ci;
        }

        internal static void HandleOnShippedEvent(int priceAmount)
        {
            EClass.game.quests.list.ForeachReverse(delegate (Quest q)
            {
                q.OnShippedMod(priceAmount);
            });
        }
    }
}
