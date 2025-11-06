using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RandomQuestExpantion.Patch
{
    // QuestTaskに独自のイベントハンドラを実装する
    [HarmonyPatch]
    class AIShoppingPatch
    {

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(AI_Shopping), nameof(AI_Shopping.Buy))]
        internal static IEnumerable<CodeInstruction> HandleOnSoldMerchandisePatch(IEnumerable<CodeInstruction> instructions)
        {
            var ci = new CodeMatcher(instructions)
                .MatchEndForward
                (
                    new CodeMatch(OpCodes.Ldloc_2),
                    new CodeMatch(OpCodes.Ldc_I4_0),
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Card), "set_isSale"))


                    //IL_0072: ldloc.2
                    //IL_0073: ldc.i4.0
                    //IL_0074: callvirt instance void Card::set_isSale(bool)           
                )
                .InsertAndAdvance
                (
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AIShoppingPatch), nameof(HandleOnSoldMerchandiseOrSlave)))
                )
                .InstructionEnumeration();
            return ci;
        }

        internal static void HandleOnSoldMerchandiseOrSlave(Card merchandise)
        {
            // AI_Shopping.Buyは奴隷売りがセットになっている
            if (merchandise.isChara)
            {
                EClass.game.quests.list.ForeachReverse(delegate (Quest q)
                {
                    q.OnSoldSlaveMod(merchandise.Chara);
                });
            }
            else
            {
                EClass.game.quests.list.ForeachReverse(delegate (Quest q)
                {
                    q.OnSoldMerchandiseMod(merchandise.Thing);
                });
            }
        }
    }
}
