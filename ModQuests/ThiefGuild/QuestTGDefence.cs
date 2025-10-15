using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuests.Common;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.ThiefGuild
{
    class QuestTGDefence : QuestUrbanDefence
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override string IdZone => "instance_guild_thief";
        public override int BaseMoney => source.money + EClass.curve(DangerLv, 500, 2000, 90);

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventTGDefence();
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 3 + EClass.rnd(2) + EClass.rnd(2);
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_thief").SetNum(guilpoNum);
            DropReward(guilpo);
            ThiefGuildZone.ModInfluence(3);
            FighterGuildZone.ModInfluence(-1);
        }
    }
}
