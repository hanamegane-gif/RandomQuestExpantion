using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuestZoneInstance;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestFarmFieldWar : QuestDefenseGame
    {
        // これは序盤向けクエストなので報酬は安め
        public override bool FameContent => false;

        public override string IdZone => "instance_harvest";
        public override string RewardSuffix => "_byakko_mod_field_war";
        public override int FameOnComplete => (difficulty * 7) * (100 + bonus / 20) / 100;



        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceFarmFieldWar();
        }

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventFarmFieldWar();
        }
        public override void OnBeforeComplete()
        {
            bonusMoney += bonus;
        }

        public override int GetRewardPlat(int money)
        {
            return 2 + EClass.rnd(2);
        }

        public override string GetTextProgress()
        {
            ZoneEventFarmFieldWar @event = EClass._zone.events.GetEvent<ZoneEventFarmFieldWar>();
            if (@event == null)
            {
                return "";
            }
            return "byakko_mod_progress_farmfield_war".lang(@event.kills.ToString() ?? "", @event.KillRequirements.ToString() ?? "");
        }
    }
}
