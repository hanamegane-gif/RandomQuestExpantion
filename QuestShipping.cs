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
