using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuests.Common;
using RandomQuestExpantion.ModQuestZoneInstance;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.FighterGuild
{
    class QuestFGDuel : QuestDuel
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public Zone ReturnZone => FighterGuildZone;

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventFGDuel();
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

            int guilpoNum = 2 + EClass.rnd(2);
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_fighter").SetNum(guilpoNum);
            DropReward(guilpo);
            FighterGuildZone.ModInfluence(1);
        }
    }
}
