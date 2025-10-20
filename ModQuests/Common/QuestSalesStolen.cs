using RandomQuestExpantion.ModQuestTask;
using RandomQuestExpantion.Patch;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestSalesStolen : QuestRandom
    {
        public override string RefDrama2 => Requirement.ToString();

        public override string TextExtra2 => "noDeadLine".lang();

        public override int KarmaOnFail => -1;

        public override string RewardSuffix => "";

        public override int RangeDeadLine => 20;

        public int Requirement => ((TaskSalesStolen)task).SoldAmountRequirements;

        public override void OnInit()
        {
            SetTask(new TaskSalesStolen());
        }

        public override void OnStart()
        {
            deadline = 0;
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
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
