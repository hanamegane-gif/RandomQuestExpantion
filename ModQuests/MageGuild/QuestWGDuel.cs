using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuests.Common;
using RandomQuestExpantion.ModQuestZoneInstance;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.MageGuild
{
    class QuestWGDuel : QuestDuel
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public Zone ReturnZone => MageGuildZone;

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventWGDuel();
        }

        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceDuel();
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 2 + EClass.rnd(2) + EClass.rnd(2);
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_mage").SetNum(guilpoNum);
            DropReward(guilpo);
            MageGuildZone.ModInfluence(1);
        }
    }
}
