using HarmonyLib;
using System;
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
                .MatchEndForward
                (
                    new CodeMatch(OpCodes.Ldfld),
                    new CodeMatch(OpCodes.Ldloc_3),
                    new CodeMatch(OpCodes.Add),
                    new CodeMatch(OpCodes.Stfld)

                //IL_02b7: ldfld int64 Player/Stats::shipMoney
                //IL_02bc: ldloc.3
                //IL_02bd: add
                //IL_02be: stfld int64 Player/Stats::shipMoney
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

        internal static void HandleOnShippedEvent(long priceAmount)
        {
            int pa = (int)(priceAmount > Int32.MaxValue ? Int32.MaxValue : priceAmount);

            EClass.game.quests.list.ForeachReverse(delegate (Quest q)
            {
                q.OnShippedMod(pa);
            });
        }
    }
}
