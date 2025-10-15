using RandomQuestExpantion.ModQuests.Common;
using RandomQuestExpantion.ModQuestTask;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.FighterGuild
{
    class QuestFGDungeonAttack : QuestDungeonAttack
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override void OnInit()
        {
            SetTask(new TaskFGDungeonAttack());
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 2 + EClass.rnd(2);
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_fighter").SetNum(guilpoNum);
            DropReward(guilpo);
            FighterGuildZone.ModInfluence(1);
        }
    }
}
