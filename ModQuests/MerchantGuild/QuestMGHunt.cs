using RandomQuestExpantion.ModQuestTask;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.MerchantGuild
{
    class QuestMGHunt : QuestHunt
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override void OnInit()
        {
            SetTask(new TaskMGHunt());
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 1;
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_merchant").SetNum(guilpoNum);
            DropReward(guilpo);
            MerchantGuildZone.ModInfluence(1);
        }
    }
}
