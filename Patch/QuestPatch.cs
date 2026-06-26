using HarmonyLib;
using RandomQuestExpantion.ModQuests;
using RandomQuestExpantion.ModQuests.QuestAttribute;

namespace RandomQuestExpantion.Patch
{
    // クエストインスタンス生成を独自で実装する
    // (バニラの生成方法だとネタかぶりが怖い)
    // 受注時に期限が延長されるクエストの期限の記述を変更
    [HarmonyPatch]
    class QuestPatch
    {
        [HarmonyPatch(typeof(Quest), nameof(Quest.Create)), HarmonyPrefix]
        internal static bool QuestCreatePatch(ref Quest __result, ref string _id, ref string _idPerson, ref Chara c)
        {
            if (!_id.Contains("byakko_mod"))
            {
                return true;
            }

            var quest = QuestFactory.CreateQuestInstance(_id, _idPerson, c);
            __result = quest;
            return false;
        }

        [HarmonyPatch(typeof(Quest), "get_TextDeadline"), HarmonyPrefix]
        internal static bool TextDeadLinePatch(Quest __instance, ref string __result)
        {
            if (__instance is IExtendDeadline q && !EClass.game.quests.list.Contains(__instance))
            {
                __result = q.GetAltTextDeadline();
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
