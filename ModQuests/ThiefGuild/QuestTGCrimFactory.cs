using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuests.Common;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.ThiefGuild
{
    class QuestTGCrimFactory : QuestCrimFactory
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventTGCrimFactory();
        }
        public override string GetTextProgress()
        {
            return "byakko_mod_progress_crim_produce".lang(Lang._weight(weightDelivered), Lang._weight(destWeight));
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 1 + EClass.rnd(2);
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_thief").SetNum(guilpoNum);
            DropReward(guilpo);
            ThiefGuildZone.ModInfluence(1);
        }
    }
}
