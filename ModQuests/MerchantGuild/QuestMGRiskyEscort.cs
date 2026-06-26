
using RandomQuestExpantion.ModQuests.Common;
using RandomQuestExpantion.ModZonePreenter;
using System.Collections.Generic;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.MerchantGuild
{
    public class QuestMGRiskyEscort : QuestRiskyEscort
    {
        internal override HashSet<string> SpawnCandidateList => new HashSet<string>
        {
            "merchant_app",
            "merchant_black",
            "merchant_slave",
            "merchant_exotic",
            "parttimer_jure",
            "rich",
        };

        public override string RewardSuffix => "_byakko_mod_guild";

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnWildernessEncounted(Zone newZone)
        {
            TravelEncounterdCount++;
            newZone.events.AddPreEnter(new ZonePreEnterMGEscortAssassin(this));
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 2;
            var guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_merchant").SetNum(guilpoNum);
            DropReward(guilpo);
            MerchantGuildZone.ModInfluence(1);
        }
    }
}
