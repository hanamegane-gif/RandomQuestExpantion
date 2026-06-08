using RandomQuestExpantion.ModQuests.Common;
using RandomQuestExpantion.ModZonePreenter;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.ThiefGuild
{
    class QuestTGRiskyDliver : QuestRiskyDeliver
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override void OnWildernessEncounted(Zone newZone)
        {
            TravelEncounterdCount++;
            newZone.events.AddPreEnter(new ZonePreEnterTGDeliverBrigand(Distribution));
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

        public override Thing GenerateDistribution()
        {
            var thing = ThingGen.Create(idThing);
            thing.Identify(show: false, IDTSource.SuperiorIdentify);

            return thing;
        }
    }
}
