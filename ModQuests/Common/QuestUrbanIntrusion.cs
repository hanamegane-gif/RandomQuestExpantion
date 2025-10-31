using RandomQuestExpantion.ModQuestZoneInstance;
using RandomQuestExpantion.ModQuestEvent;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestUrbanIntrusion : QuestSubdue
    {
        public override string IdZone => "instance_" + this.chara.currentZone.id;

        public override int BaseMoney => source.money + EClass.curve(DangerLv, 500, 2000, 90) * 2;

        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceUrbanIntrusion();
        }

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventUrbanIntrusion();
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2) + EClass.curve(DangerLv / 25, 10, 20, 80);
        }
    }
}
