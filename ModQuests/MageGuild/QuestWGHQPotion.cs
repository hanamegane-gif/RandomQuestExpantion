using RandomQuestExpantion.ModQuests.Common;
using UnityEngine;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.MageGuild
{
    class QuestWGHQPotion : QuestHQCraft
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override int GetBonus(Thing t)
        {
            return 0;
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        internal override void SetTargetCategory()
        {
            // ポーション(バニラでは依頼に使えるのは錬金ポーションのみ)
            idCat = "_drink";
        }

        internal override void SetQualityRequirement()
        {
            QualityLvRequirement = Mathf.Clamp((EClass.pc.FameLv / 10) * 10, 20, 50);
        }

        internal override void SetAttributeRequirement()
        {
            // 癒し(750),珍しさ(751),見た目(752),毒消し(753),安定作用(754),止血(755),刺激(760),空気(763)から出題する
            // 特性は確保が難しいので10とする
            ElementIdRequirement = new int[] { 750, 751, 752, 753, 754, 755, 760, 763 }.RandomItem();
            ElementLvRequirement = 10;
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 1 + EClass.rnd(2) + EClass.rnd(2);
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_mage").SetNum(guilpoNum);
            DropReward(guilpo);
            MageGuildZone.ModInfluence(1);
        }
    }
}
