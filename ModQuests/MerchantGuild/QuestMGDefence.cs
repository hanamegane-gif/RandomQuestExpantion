using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuests.Common;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.MerchantGuild
{
    class QuestMGDefence : QuestUrbanDefence
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override string IdZone => "instance_guild_merchant";
        public override int BaseMoney => source.money + EClass.curve(DangerLv, 500, 2000, 90);

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventMGDefence();
        }

        public override int GetRewardPlat(int money)
        {
            return 4 + EClass.rnd(2);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 3 + EClass.rnd(2) + EClass.rnd(2);
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_merchant").SetNum(guilpoNum);
            DropReward(guilpo);
            MerchantGuildZone.ModInfluence(3);
            ThiefGuildZone.ModInfluence(-1);
        }
    }
}
