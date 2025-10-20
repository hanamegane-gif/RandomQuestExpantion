using RandomQuestExpantion.ModQuestTask;
using RandomQuestExpantion.Patch;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestDungeonAttack : QuestHuntRace
    {
        public override void OnInit()
        {
            SetTask(new TaskDungeonAttack());
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2) + curve(bonusMoney / 400, 6, 10, 75);
        }

        public virtual void OnNefiaBeaten(in Chara boss)
        {
            if (task != null)
            {
                task.OnNefiaBeatenMod(boss);
                if (task.IsComplete())
                {
                    CompleteTask();
                }
            }
        }
    }
}
