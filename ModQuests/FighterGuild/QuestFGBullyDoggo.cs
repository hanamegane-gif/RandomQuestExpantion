using RandomQuestExpantion.Config;
using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuests.Common;
using RandomQuestExpantion.ModQuestTask;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.FighterGuild
{
    public class QuestFGBullyDoggo : QuestBullyDoggo
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        public override void OnInit()
        {
            SetTask(new TaskFGBullyDoggo());
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnStart()
        {
            var doggo = SpawnDoggo();
            UIDDoggo = doggo.uid;
            (task as TaskFGBullyDoggo).SetUIDDoggo(doggo.uid);

            var woofEvent = new ZoneEventFGBullyDoggo();
            woofEvent.SetOwnerQuest(this);
            woofEvent.SetUIDDoggo(doggo.uid);
            EClass._zone.events.Add(woofEvent);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 2 + EClass.rnd(2);
            var guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_fighter").SetNum(guilpoNum);
            DropReward(guilpo);
            FighterGuildZone.ModInfluence(1);
        }
    }
}
