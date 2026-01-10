using RandomQuestExpantion.ModQuestTask;
using RandomQuestExpantion.Patch;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestShipping : QuestRandom
    {
        public override string RefDrama2 => Requirement.ToString();

        public override string TextExtra2 => "noDeadLine".lang();

        public override int KarmaOnFail => -1;

        public override string RewardSuffix => "";

        public override int RangeDeadLine => 20;

        public int Requirement => ((TaskShipping)task).ShippedAmountRequirements;

        public override void OnInit()
        {
            SetTask(new TaskShipping());
        }

        public override void OnStart()
        {
            deadline = 0;
        }

        public override void OnDropReward()
        {
            int num = bonusMoney * (55 + difficulty * 15) / 100;
            int num2 = rewardMoney + num;
            if (num2 > 0)
            {
                if (num > 0)
                {
                    Msg.Say("reward_bonus", num.ToString() ?? "");
                }

                DropReward(ThingGen.CreateCurrency(num2));
            }

            if (EClass._zone != null && !EClass._zone.IsRegion)
            {
                Zone zone = EClass._zone.GetTopZone();
                if ((!zone.IsTown || zone.IsPCFaction) && base.ClientZone != null)
                {
                    Zone topZone = base.ClientZone.GetTopZone();
                    if (topZone.IsTown && !topZone.IsPCFaction)
                    {
                        zone = topZone;
                    }
                    if (zone.IsTown || zone.IsPCFaction)
                    {
                        zone.GetTopZone().ModInfluence(1);
                    }
                }
            }


            Thing thing = ThingGen.Create("plat").SetNum(GetRewardPlat(num2));
            DropReward(thing);

            if (FameOnComplete > 0)
            {
                EClass.player.ModFame(EClass.rndHalf(FameOnComplete));
            }
        }

        public virtual void OnShipped(int priceAmount)
        {
            if (task != null)
            {
                task.OnShippedMod(priceAmount);
                if (task.IsComplete())
                {
                    CompleteTask();
                }
            }
        }
    }
}
