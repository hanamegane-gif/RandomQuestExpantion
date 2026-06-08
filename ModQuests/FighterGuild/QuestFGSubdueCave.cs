using RandomQuestExpantion.ModQuestEvent;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.FighterGuild
{
    class QuestFGSubdueCave : QuestSubdue
    {
        public override string IdZone => "instance_cave0";

        public override string RewardSuffix => "_byakko_mod_guild";

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventFGSubdueCave();
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 2 + EClass.rnd(2) + EClass.rnd(2);
            var guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_fighter").SetNum(guilpoNum);
            DropReward(guilpo);
            FighterGuildZone.ModInfluence(1);
        }
    }
}
