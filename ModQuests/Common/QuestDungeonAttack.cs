using RandomQuestExpantion.ModQuestTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return 1 + EClass.rnd(2) + curve(bonusMoney, 20, 20, 50);
        }
    }
}
