using HarmonyLib;
using RandomQuestExpantion.ModNefia;

namespace RandomQuestExpantion.Patch
{
    // Questに独自のイベントハンドラを実装する
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

        [HarmonyPatch(typeof(TraitNewZone), nameof(TraitNewZone.CreateZone)), HarmonyPostfix]
        public static void InheritPatch(Zone __result)
        {
            if (__result is IQuestRandomNefia)
            {
                (__result as IQuestRandomNefia).RevertToVanillaZoneId();
            }
        }
    }
}
