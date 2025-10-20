using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.MerchantGuild
{
    class QuestMGDeliver : QuestDeliver
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        // テレポーターの使用は認められてもいいと思っている
        // 楽をするために必要なものを投資しているわけだし
        public override bool ForbidTeleport => false;

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
