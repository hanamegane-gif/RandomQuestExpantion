using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.Patch
{
    [HarmonyPatch]
    internal class Zone_UpdateQuestPatch
    {
        // Type毎のchanceの合計値キャッシュ
        private static Dictionary<string, int> ChanceSumChache = new Dictionary<string, int>();

        // Type毎のchanceの合計値に対する個別クエストのchanceの割合キャッシュ
        private static Dictionary<string, int> ChanceRatioChache = new Dictionary<string, int>();

        [HarmonyTargetMethod]
        internal static MethodBase TargetMethod()
        {
            return typeof(Zone).GetNestedTypes((BindingFlags)60)
                               .SelectMany(t => t.GetMethods((BindingFlags)60))
                               .First(m =>
                                    m.Name.StartsWith("<UpdateQuests>b__") &&
                                    m.ReturnType == typeof(float) &&
                                    m.GetParameters().Length == 1 &&
                                    m.GetParameters()[0].ParameterType == typeof(SourceQuest.Row)
                               );
        }

        [HarmonyPrefix]
        internal static bool QuestLotteryRatePatch(object __instance, SourceQuest.Row a, ref float __result)
        {
            var listTag = (List<Tuple<string, int>>)__instance.GetType().GetField("listTag").GetValue(__instance);

            if (!ChanceSumChache.Any())
            {
                CreateChanceSumCache();
            }

            if (!ChanceRatioChache.ContainsKey(a.id))
            {
                AddChanceRatioCache(a);
            }

            int chanceMultiplier = 1;
            foreach (var tag in listTag)
            {
                if (a.tags.Contains(tag.Item1))
                {
                    chanceMultiplier *= tag.Item2;
                }
            }

            if (!EClass._zone.IsPCFaction && a.tags.Contains("bulk"))
            {
                chanceMultiplier = 0;
            }

            if (IsGuild(EClass._zone) && !IsGuildQuest(a))
            {
                chanceMultiplier = 0;
            }

            __result = ChanceRatioChache[a.id] * chanceMultiplier;

            return false;
        }

        internal static void CreateChanceSumCache()
        {
            // 最初のタグだけ取る(2番目はneedDestZoneとかの種別じゃないやつになる)
            HashSet<string> tagList = EClass.sources.quests.rows.Where(r => r.tags.Any()).Select(r => r.tags[0]).ToHashSet();

            foreach (var tag in tagList)
            {
                int chanceSum = EClass.sources.quests.rows.Where(r => r.tags.Contains(tag)).Sum(r => r.chance);
                ChanceSumChache.Add(tag, chanceSum);
            }
        }

        internal static void AddChanceRatioCache(SourceQuest.Row questRow)
        {
            int chanceRatio = 0; 
            var matchList = ChanceSumChache.Where(kvp => questRow.tags.Contains(kvp.Key)).Select(kvp => kvp.Value).ToList();

            if (matchList.Any())
            {
                foreach (var sum in matchList)
                {
                    chanceRatio += 10000 * questRow.chance / sum;
                }
            }
            else
            {
                chanceRatio = 10000;
            }

            ChanceRatioChache.Add(questRow.id, chanceRatio);
        }

        internal static bool IsGuildQuest(SourceQuest.Row questRow)
        {
            return questRow.tags.Any(t => (t == "fighter" || t == "mage" || t == "thief" || t == "merchant"));
        }
    }
}
