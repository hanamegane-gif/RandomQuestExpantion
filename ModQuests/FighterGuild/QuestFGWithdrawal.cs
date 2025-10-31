using RandomQuestExpantion.ModQuestEvent;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.FighterGuild
{
    class QuestFGWithdrawal : QuestDefenseGame
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public Zone ReturnZone => FighterGuildZone;

        public override bool FameContent => true;

        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceDefense();
        }

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventFGWithdrawal();
        }

        public override int GetRewardPlat(int money)
        {
            return 3 + EClass.rnd(2);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 3 + EClass.rnd(2) + EClass.rnd(2);
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_fighter").SetNum(guilpoNum);
            DropReward(guilpo);
            FighterGuildZone.ModInfluence(3);
        }

        public override string GetTextProgress()
        {
            ZoneEventFGWithdrawal @event = EClass._zone.events.GetEvent<ZoneEventFGWithdrawal>();
            if (@event == null)
            {
                return "";
            }
            return "byakko_mod_progress_withdrawal".lang(@event.wave.ToString() ?? "", @event.CanRetreatWave.ToString() ?? "");
        }
    }
}
