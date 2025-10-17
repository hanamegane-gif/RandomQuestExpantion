using RandomQuestExpantion.ModQuestZoneInstance;
using RandomQuestExpantion.ModQuestEvent;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestUrbanBoss : QuestSubdue
    {
        public override string IdZone => "instance_" + this.chara.currentZone.id;
        public override int BaseMoney => source.money + EClass.curve(DangerLv, 500, 2000, 90) * 5;


        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceUrbanBoss();
        }

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventUrbanBoss();
        }


        public override string GetTextProgress()
        {
            ZoneEventUrbanBoss @event = EClass._zone.events.GetEvent<ZoneEventUrbanBoss>();
            if (@event == null)
            {
                return "";
            }

            if (@event.IsInInterval)
            {
                return "byakko_mod_progress_urban_boss_interval".lang(@event.RemainWaves.ToString());
            }
            else if(@event.IsEngaging)
            {
                return "byakko_mod_progress_urban_boss_engage".lang(@event.CurrentBoss.NameBraced);
            }
            return "";
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2) + curve(bonusMoney / 60, 20, 20, 60);
        }
    }
}
