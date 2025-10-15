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
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(List<Card>), nameof(List<Card>.Remove))),
                    new CodeMatch(OpCodes.Pop)

                    //IL_008c: ldloc.2
                    //IL_008d: callvirt instance bool class [mscorlib] System.Collections.Generic.List`1<class Card>::Remove(!0)
                    //IL_0092: pop
                )
                .Advance(1)
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
