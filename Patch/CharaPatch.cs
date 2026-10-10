using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RandomQuestExpantion.Patch
{
    // QuestTaskに独自のイベントハンドラを実装する
    // モンスターを演奏依頼中に限り聴衆になれるようにする
    [HarmonyPatch]
    class CharaPatch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Chara), nameof(Chara.TryDropBossLoot))]
        internal static IEnumerable<CodeInstruction> HandleNefiaBeatenPatch(IEnumerable<CodeInstruction> instructions)
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

        [HarmonyPatch(typeof(Chara), "get_CanWitness"), HarmonyPrefix]
        internal static bool CanWitnessPatch(ref bool __result)
        {
            if (EClass._zone is Zone_Music)
            {
                __result = true;
                return false;
            }

            return true;
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
