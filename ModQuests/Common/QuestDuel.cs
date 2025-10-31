using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuestZoneInstance;

namespace RandomQuestExpantion.ModQuests.Common
{
    public class QuestDuel : QuestSubdue
    {
        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventDuel();
        }

        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceDuel();
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2) + EClass.curve(DangerLv / 25, 10, 20, 80);
        }
    }
}
