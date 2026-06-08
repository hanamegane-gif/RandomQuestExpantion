using RandomQuestExpantion.ModQuestTask;
using RandomQuestExpantion.ModQuests.Common;

namespace RandomQuestExpantion.ModQuests.FighterGuild
{
    class QuestFGBullyDoggo : QuestBullyDoggo
    {
        public override void OnInit()
        {
            SetTask(new TaskFGBullyDoggo());
        }
    }
}
