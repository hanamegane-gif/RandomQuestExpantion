using HarmonyLib;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.Patch
{
    // 通常は市民しかクエストを発注できないためギルド員(見習い商人とか戦士とか)がクエストを発注できるようにする
    [HarmonyPatch]
    class TraitCharaPatch
    {
        [HarmonyPatch(typeof(TraitChara), "get_CanGiveRandomQuest"), HarmonyPrefix]
        internal static bool FriendlyGuildPatch(ref bool __result, TraitChara __instance)
        {
            if (IsGuild(__instance.owner.currentZone))
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
}
