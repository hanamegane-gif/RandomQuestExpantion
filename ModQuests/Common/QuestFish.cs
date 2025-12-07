using RandomQuestExpantion.ModQuestEvent;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestFish : QuestHarvest
    {
        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventFish();
        }
    }
}
