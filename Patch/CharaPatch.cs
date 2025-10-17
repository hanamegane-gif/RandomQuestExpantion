using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RandomQuestExpantion.Patch
{
    [HarmonyPatch]
    class CharaPatch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Chara), nameof(Chara.TryDropBossLoot))]
        internal static IEnumerable<CodeInstruction> HandleOnShippedEventPatch(IEnumerable<CodeInstruction> instructions)
        {
            var ci = new CodeMatcher(instructions)
                .MatchStartForward
                (
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Player.Stats), "set_nefiaBeaten"))

                    //IL_00ec: callvirt instance void Player / Stats::set_nefiaBeaten(int64)
                )
                .Advance(1)
                .InsertAndAdvance
                (
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CharaPatch), nameof(HandleOnNefiaBeaten)))
                )
                .InstructionEnumeration();
            return ci;
        }

        internal static void HandleOnNefiaBeaten(Chara boss)
        {
            EClass.game.quests.list.ForeachReverse(delegate (Quest q)
            {
                q.OnNefiaBeatenMod(boss);
            });
        }

    }
}
