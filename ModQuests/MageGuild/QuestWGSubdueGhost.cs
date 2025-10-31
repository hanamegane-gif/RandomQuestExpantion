using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuestZoneInstance;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.MageGuild
{
    class QuestWGSubdueGhost : QuestSubdue
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override string IdZone => "instance_music";

        public override int BaseMoney => source.money + EClass.curve(DangerLv, 20, 15) * 10;

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventWGSubdueGhost();
        }

        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceSubdueGhost();
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 2 + EClass.rnd(2);
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_mage").SetNum(guilpoNum);
            DropReward(guilpo);
            MageGuildZone.ModInfluence(1);
        }
    }
}
