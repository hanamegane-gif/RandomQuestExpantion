using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuestZoneInstance;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestUrbanDefence : QuestSubdue
    {
        public override string IdZone => "instance_" + this.chara.currentZone.id;

        public override int BaseMoney => source.money + EClass.curve(DangerLv, 500, 2000, 90) * 3;

        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceUrbanDefence();
        }

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventUrbanDefence();
        }

        public override string GetTextProgress()
        {
            if (isComplete)
            {
                return "";
            }

            return "byakko_mod_progress_urban_defence".lang();
        }
    }
}
