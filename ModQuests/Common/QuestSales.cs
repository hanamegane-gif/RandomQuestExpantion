using RandomQuestExpantion.ModQuestTask;
using RandomQuestExpantion.Patch;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestSales : QuestRandom
    {
        public override string RefDrama2 => Requirement.ToString();

        public override string TextExtra2 => "noDeadLine".lang();

        public override int KarmaOnFail => -1;

        public override string RewardSuffix => "";

        public override int RangeDeadLine => 20;

        public int Requirement => ((TaskSales)task).SoldAmountRequirements;

        public override void OnInit()
        {
            SetTask(new TaskSales());
        }

        public override void OnStart()
        {
            deadline = 0;
        }

        public virtual void OnSoldMerchandise(in Thing merchandise)
        {
            if (task != null)
            {
                task.OnSoldMerchandiseMod(merchandise);
                if (task.IsComplete())
                {
                    CompleteTask();
                }
            }
        }
    }
}
