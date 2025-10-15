using RandomQuestExpantion.ModQuests.Common;
using RandomQuestExpantion.ModQuestTask;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.MerchantGuild
{
    class QuestMGSales : QuestSales
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override void OnInit()
        {
            SetTask(new TaskMGSales());
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 2;
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_merchant").SetNum(guilpoNum);
            DropReward(guilpo);
            MerchantGuildZone.ModInfluence(1);
        }
    }
}
