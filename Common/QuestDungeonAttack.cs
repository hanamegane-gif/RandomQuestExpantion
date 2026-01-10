using RandomQuestExpantion.Config;
using RandomQuestExpantion.ModQuestTask;
using RandomQuestExpantion.Patch;
using UnityEngine;

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
            int bonusPlat = (ModConfig.RewardPlatRate > 0) ? EClass.curve(bonusMoney / 400, 6, 10, 75) * ModConfig.RewardPlatRate / 100 : EClass.rndHalf((int)Mathf.Sqrt(money / 200));
            return 1 + EClass.rnd(2) + bonusPlat;
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
