using HarmonyLib;
using RandomQuestExpantion.ModQuests;

namespace RandomQuestExpantion.Patch
{
    // クエストインスタンス生成を独自で実装する
    // (バニラの生成方法だとネタかぶりが怖い)
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

            Quest quest = QuestFactory.CreateQuestInstance(_id, _idPerson, c);
            __result = quest;
            return false;
        }
    }
}
