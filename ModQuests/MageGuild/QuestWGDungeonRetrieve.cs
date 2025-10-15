using RandomQuestExpantion.ModQuests.Common;
using RandomQuestExpantion.ModQuestTask;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.MageGuild
{
    class QuestWGDungeonRetrieve : QuestDungeonRetrieve
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override void OnInit()
        {
            SetTask(new TaskWGDungeonRetrieve());
            num = GetDestNum();
            SetIdThing();
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 2 + EClass.rnd(2) + EClass.rnd(2);
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_mage").SetNum(guilpoNum);
            DropReward(guilpo);
            MageGuildZone.ModInfluence(1);
        }
    }
}
