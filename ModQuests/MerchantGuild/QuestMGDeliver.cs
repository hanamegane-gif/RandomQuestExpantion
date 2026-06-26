using RandomQuestExpantion.ModQuests.QuestAttribute;
using UnityEngine;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.MerchantGuild
{
    public class QuestMGDeliver : QuestDeliver, IExtendDeadline
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public virtual int DaysExtraDeadline => 2 + (7 - Mathf.Clamp(this.difficulty, 1, 7)) / 2;

        // テレポーターの使用は認められてもいいと思っている
        // 楽をするために必要なものを投資しているわけだし
        public override bool ForbidTeleport => false;

        public override void OnStart()
        {
            base.OnStart();
            OnStartExtendDeadline();
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 1;
            var guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_merchant").SetNum(guilpoNum);
            DropReward(guilpo);
            MerchantGuildZone.ModInfluence(1);
        }

        public Quest OnStartExtendDeadline()
        {
            deadline += DaysExtraDeadline * Date.DayToken;
            return this;
        }

        public string GetAltTextDeadline()
        {
            return AltTextDeadline((Hours >= 0) ? Hours : 0, DaysExtraDeadline);
        }
    }
}
