using HarmonyLib;
using RandomQuestExpantion.ModQuests.Common;
using System.Linq;

namespace RandomQuestExpantion.Patch
{
    // 収穫依頼の納品物に新規パターンを追加する
    [HarmonyPatch]
    class InvOwnerDeliverPatch
    {
        [HarmonyPatch(typeof(InvOwnerDeliver), nameof(InvOwnerDeliver.ShouldShowGuide)), HarmonyPrefix]
        internal static bool NewDeliverModePatch(InvOwnerDeliver __instance, ref bool __result, Thing t)
        {
            // 通常は同時に2つ以上受けられないため1つ取得すればよい
            var quest = EClass.game.quests.list.Where(q => q is IHarvest).Cast<IHarvest>().FirstOrDefault();
            if (quest != null)
            {
                __result = quest.IsQuestItem(t);
                return false;
            }

            return true;
        }
    }
}
