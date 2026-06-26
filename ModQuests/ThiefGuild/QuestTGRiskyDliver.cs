using RandomQuestExpantion.ModQuests.Common;
using RandomQuestExpantion.ModZonePreenter;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.ThiefGuild
{
    public class QuestTGRiskyDliver : QuestRiskyDeliver
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override void OnWildernessEncounted(Zone newZone)
        {
            TravelEncounterdCount++;
            newZone.events.AddPreEnter(new ZonePreEnterTGDeliverBrigand(this));
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 2 + EClass.rnd(2) + EClass.rnd(2);
            var guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_thief").SetNum(guilpoNum);
            DropReward(guilpo);
            ThiefGuildZone.ModInfluence(1);
        }

        public override void OnFail()
        {
            base.OnFail();

            // リロードがしやすい依頼のため、影響度でバランスを取ろうとする
            ThiefGuildZone.ModInfluence(-2);
        }

        internal override Thing GenerateDistribution()
        {
            var thing = base.GenerateDistribution();
            thing.isStolen = true;

            return thing;
        }
    }
}
