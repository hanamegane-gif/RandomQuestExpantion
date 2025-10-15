using RandomQuestExpantion.ModQuestTask;
using RandomQuestExpantion.Patch;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestSlaver : QuestRandom
    {
        public override string RefDrama2 => Requirement.ToString();

        public override string TextExtra2 => "noDeadLine".lang();

        public override int KarmaOnFail => -1;

        public override string RewardSuffix => "";

        public override int RangeDeadLine => 20;

        public int Requirement => ((TaskSlaver)task).SoldAmountRequirements;

        public override void OnInit()
        {
            SetTask(new TaskSlaver());
        }

        public override void OnStart()
        {
            deadline = 0;
        }

        public virtual void OnSoldSlave(Chara slave)
        {
            RandomQuestExpantion.Log("OnSoldSlave2");

            if (task != null)
            {
                task.OnSoldSlaveMod(slave);
                if (task.IsComplete())
                {
                    CompleteTask();
                }
            }
        }
    }
}
