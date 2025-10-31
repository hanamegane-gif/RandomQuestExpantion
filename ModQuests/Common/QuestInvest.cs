using RandomQuestExpantion.ModQuestTask;
using RandomQuestExpantion.Patch;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestInvest : QuestRandom
    {
        public override string RefDrama2 => Requirement.ToString();

        public override int KarmaOnFail => -1;

        public override string RewardSuffix => "";

        public override int RangeDeadLine => 20;

        public int Requirement => ((TaskInvest)task).InvestRequirements;

        public override void OnInit()
        {
            SetTask(new TaskInvest());
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public virtual void OnInvest()
        {
            if (task != null)
            {
                task.OnInvestMod();
                if (task.IsComplete())
                {
                    CompleteTask();
                }
            }
        }
    }
}
