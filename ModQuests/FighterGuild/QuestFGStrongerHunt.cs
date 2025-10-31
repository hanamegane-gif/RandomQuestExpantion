using RandomQuestExpantion.ModQuests.MerchantGuild;
using RandomQuestExpantion.ModQuestTask;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.FighterGuild
{
    class QuestFGStrongerHunt : QuestFGHunt
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override bool FameContent => true;

        public override void OnInit()
        {
            SetTask(new TaskFGStrongerHunt());
        }

        public override int GetRewardPlat(int money)
        {
            return 2 + EClass.rnd(2);
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
