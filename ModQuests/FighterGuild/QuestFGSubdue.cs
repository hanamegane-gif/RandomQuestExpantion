using RandomQuestExpantion.ModQuestEvent;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.FighterGuild
{
    class QuestFGSubdue : QuestSubdue
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public Zone ReturnZone => FighterGuildZone;

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventFGSubdue();
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 2 + EClass.rnd(2) + EClass.rnd(2);
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_fighter").SetNum(guilpoNum);
            DropReward(guilpo);
            FighterGuildZone.ModInfluence(1);
        }
    }
}
