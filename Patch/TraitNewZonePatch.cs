using HarmonyLib;

namespace RandomQuestExpantion.Patch
{
    [HarmonyPatch]
    class TraitNewZonePatch
    {
        [HarmonyPatch(typeof(TraitNewZone), nameof(TraitNewZone.MoveZone)), HarmonyPrefix]
        internal static void HandleOnNefiaRetreatedPatch(bool confirmed)
        {
            if (confirmed && EClass._zone.IsNefia && EClass._zone.Boss != null)
            {
                EClass.game.quests.list.ForeachReverse(delegate (Quest q)
                {
                    q.OnNefiaRetreatedMod(EClass._zone);
                });
            }
        }
    }
}
