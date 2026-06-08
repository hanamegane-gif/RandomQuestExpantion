
using RandomQuestExpantion.ModQuests.Common;
using RandomQuestExpantion.ModZonePreenter;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.MerchantGuild
{
    class QuestMGRiskyEscort : QuestRiskyEscort
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnWildernessEncounted(Zone newZone)
        {
            TravelEncounterdCount++;
            newZone.events.AddPreEnter(new ZonePreEnterMGEscortAssassin(TargetName));
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 2;
            var guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_merchant").SetNum(guilpoNum);
            DropReward(guilpo);
            MerchantGuildZone.ModInfluence(1);
        }

        public override Chara SpawnEscortTarget()
        {
            var chara = CharaGen.Create("merchant_app");
            EClass._zone.AddCard(chara, EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false));
            chara.MakeMinion(EClass.pc);

            return chara;
        }
    }
}
