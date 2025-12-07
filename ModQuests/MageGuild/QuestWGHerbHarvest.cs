using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuests.Common;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.MageGuild
{
    class QuestWGHerbHarvest : QuestHerbHarvest
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventWGHerbHarvest();
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 1 + EClass.rnd(2);
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_mage").SetNum(guilpoNum);
            DropReward(guilpo);
            MageGuildZone.ModInfluence(1);
        }
    }
}
