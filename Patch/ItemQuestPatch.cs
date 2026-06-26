using HarmonyLib;

namespace RandomQuestExpantion.Patch
{
    [HarmonyPatch]
    internal class ItemQuestPatch
    {
        [HarmonyPatch(typeof(ItemQuest), nameof(ItemQuest.SetQuest)), HarmonyPostfix]
        internal static void TextDeadLinePatch(ItemQuest __instance, Quest q)
        {
            __instance.textDate.SetText(q.TextDeadline);
            __instance.textDate.transform.parent.RebuildLayout();
        }
    }
}
