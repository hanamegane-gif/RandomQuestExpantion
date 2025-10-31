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
            QuestHarvest quest = EClass.game.quests.list.Where(q => q is QuestHarvest && q.GetType().Assembly.GetName().Name == "RandomQuestExpantion")
                                                        .Cast<QuestHarvest>()
                                                        .FirstOrDefault();
            if (quest != null)
            {
                __result = false;

                if (quest is QuestHerbHarvest)
                {
                    __result = (quest as QuestHerbHarvest).IsQuestItem(t);
                }

                if (quest is QuestCrimFactory)
                {
                    __result = (quest as QuestCrimFactory).IsQuestItem(t);
                }

                return false;
            }

            return true;
        }
    }
}
