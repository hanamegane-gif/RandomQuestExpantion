using HarmonyLib;
using RandomQuestExpantion.ModQuests;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.Patch
{
    // ギルドから受けるインスタンスマップクエストのParentZoneがないことによるバグを抑制する
    [HarmonyPatch]
    class SpatialGenPatch
    {
        [HarmonyPatch(typeof(SpatialGen), nameof(SpatialGen.CreateInstance)), HarmonyPostfix]
        internal static void SetParentZonePatch(string id, ZoneInstance instance, ref Zone __result)
        {
            if (IsGuild(EClass._zone) || (EClass._zone is Zone_Town && EClass._zone.lv != 0))
            {
                EClass._zone.AddChild(__result);
            }
        }
    }
}
