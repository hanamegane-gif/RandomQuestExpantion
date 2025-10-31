using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.MerchantGuild
{
    class QuestMGEscort : QuestEscort
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override void OnStart()
        {
            Chara chara = CharaGen.Create("merchant_app");
            EClass._zone.AddCard(chara, EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false));
            chara.MakeMinion(EClass.pc);
            uidChara = chara.uid;
            chara.Talk("parasite", null, null, forceSync: true);
        }

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
