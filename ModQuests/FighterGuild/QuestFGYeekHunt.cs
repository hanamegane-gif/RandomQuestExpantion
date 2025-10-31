using RandomQuestExpantion.ModQuestTask;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.FighterGuild
{
    class QuestFGYeekHunt : QuestHunt
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override string TextExtra2 => "noDeadLine".lang();

        public override void OnInit()
        {
            SetTask(new TaskFGYeekHunt());
        }

        public override void OnStart()
        {
            deadline = 0;
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 1 + EClass.rnd(2);
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_fighter").SetNum(guilpoNum);
            DropReward(guilpo);
            FighterGuildZone.ModInfluence(1);
        }
    }
}
