using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.Patch
{
    // ギルドでクエストを受けられるようにする
    // ギルド専用クエストをギルドで受けられるように&ギルド専用クエストを他の場所で受けられないようにフィルタ
    // 「○○への影響度が～」の表記をギルドでも表示できるように
    [HarmonyPatch]
    class ZonePatch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Zone), nameof(Zone.UpdateQuests))]
        internal static IEnumerable<CodeInstruction> AllowGuildQuestPatch(IEnumerable<CodeInstruction> instructions)
        {
            var ci = new CodeMatcher(instructions)
                .MatchStartForward
                (
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Isinst)

                    //IL_000e: ldarg.0
                    //IL_000f: isinst Zone_Town
                    //IL_0014: brfalse.s IL_001e
                    //IL_0016: ldarg.0
                    //IL_0017: call instance int32 Spatial::get_lv()

                    //IL_001c: brfalse.s IL_001f
                    //IL_001e: ret
                )
                .Advance(1)
                .RemoveInstructions(4)
                .InsertAndAdvance
                (
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ZonePatch), nameof(IsNotTownOrGuild)))
                )
                .InstructionEnumeration();
            return ci;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Zone), nameof(Zone.UpdateQuests))]
        internal static IEnumerable<CodeInstruction> GuildQuestFilterPatch(IEnumerable<CodeInstruction> instructions)
        {
            var ci = new CodeMatcher(instructions)
                .MatchEndForward
                (
                    new CodeMatch(ci =>
                        (ci.opcode == OpCodes.Ldloc_S || ci.opcode == OpCodes.Ldloc) &&
                        (ci.operand is byte b && b == 4 || ci.operand is LocalBuilder lb && lb.LocalIndex == 4)
                    ),
                    new CodeMatch(ci =>
                        (ci.opcode == OpCodes.Stloc_S || ci.opcode == OpCodes.Stloc) &&
                        (ci.operand is byte b && b == 7 || ci.operand is LocalBuilder lb && lb.LocalIndex == 7)
                    )

                    // string[] array2 = array;
                    //IL_0211: ldloc.s 4
                    //IL_0213: stloc.s 7
                )
                .InsertAndAdvance
                (
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ZonePatch), nameof(ApplyGuildQuestFilter)))
                )
                .InstructionEnumeration();
            return ci;
        }

        [HarmonyPatch(typeof(Zone), nameof(Zone.ModInfluence)), HarmonyPrefix]
        internal static bool InfluenceGuildPatch(Zone __instance, int a)
        {
            if (!IsGuild(__instance))
            {
                return true;
            }

            string factionName = (IsFighterGuild(__instance)) ? Guild.Fighter.Name :
                                 (IsMerchantGuild(__instance)) ? Guild.Merchant.Name :
                                 (IsThiefGuild(__instance)) ? Guild.Thief.Name :
                                 (IsMageGuild(__instance)) ? Guild.Mage.Name : "";

            __instance.influence += a;
            if (__instance.influence <= 0)
            {
                __instance.influence = 0;
            }

            if (a > 0)
            {
                Msg.Say("gainInfluence", factionName, a.ToString() ?? "");
            }
            else if (a < 0)
            {
                Msg.Say("byakko_mod_lost_influence", factionName, a.ToString() ?? "");
            }
            return false;
        }

        // 街かギルド「でない」判定
        // 気持ち悪いがILに差し込む先がbrfalseなのでしかたなし
        internal static bool IsNotTownOrGuild(Zone currentZone)
        {
            if (currentZone is Zone_Town || IsInGuild())
            {
                return false;
            }

            return true;
        }

        // ギルドクエストフィルタをここで適用する
        // 商人以外のギルドは独立したZoneを持たないためExcelで設定できない
        // ギルドフィルタを新設したので全ての街で再設定する必要がある
        internal static string[] ApplyGuildQuestFilter(string[] questFilter)
        {
            var newArray = new List<string>();
            if (!IsInGuild())
            {
                newArray.AddRange(questFilter.ToList());
                newArray.AddRange(new List<string> { "fighter/0", "mage/0", "thief/0", "merchant/0" });
                return newArray.ToArray();
            }

            newArray.AddRange(new List<string>{ "supply/0", "deliver/0", "food/0", "escort/0", "deliver/0", "monster/0", "war/0", "farm/0", "music/0" });
            if (Guild.Fighter.IsCurrentZone)
            {
                newArray.AddRange(new List<string> { "fighter/999", "mage/0", "thief/0", "merchant/0" });
            }
            else if (Guild.Mage.IsCurrentZone)
            {
                newArray.AddRange(new List<string> { "fighter/0", "mage/999", "thief/0", "merchant/0" });
            }
            else if (Guild.Thief.IsCurrentZone)
            {
                newArray.AddRange(new List<string> { "fighter/0", "mage/0", "thief/999", "merchant/0" });
            }
            else if (Guild.Merchant.IsCurrentZone)
            {
                newArray.AddRange(new List<string> { "fighter/0", "mage/0", "thief/0", "merchant/999" });
            }

            return newArray.ToArray();
        }
    }
}
