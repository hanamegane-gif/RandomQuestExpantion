using RandomQuestExpantion.ModQuestEvent;

namespace RandomQuestExpantion.ModQuests.Common
{
    public class QuestFish : QuestHarvest
    {
        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventFish();
        }
    }
}
