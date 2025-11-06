using System.Collections.Generic;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.ThiefGuild
{
    class QuestTGDeliver : QuestDeliver
    {
        public override string RewardSuffix => "_byakko_mod_guild";

        // 手段は問わない
        public override bool ForbidTeleport => false;

        // 禁制品タグみたいなのはないので、なんとなく危なそうなものを手動で選ぶ
        public override void SetIdThing()
        {
            // 334: 睡眠剤
            // 837: 財宝
            // 271: 妹のいる風景
            // 272: しどけないネーネネネ
            // 273: しどけないよそルぅ！
            // 958: 謎の影
            // 1172: 途方もない
            // 1269: カレー
            var candidateList = new HashSet<string> { 
                "drug_crim", "334", "gallows", "diary_lady", 
                "statue_weird", "267", "torture_cross", "syringe_gene", 
                "syringe_heaven", "stethoscope", "butcherknife" ,"837",
                "271", "272", "273", "958",
                "mask_jason", "mask_anon", "rod_wish", "goodness",
                "1172", "1269"
            };
            idThing = candidateList.RandomItem();
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnStart()
        {
            Thing thing = ThingGen.Create(idThing);
            thing.isStolen = true;

            if (idThing == "rod_wish")
            {
                // だめにきまってんでしょ！
                thing.SetCharge(0);
            }

            thing.Identify(show: false, IDTSource.SuperiorIdentify);
            Msg.Say("get_quest_item");
            EClass.pc.Pick(thing);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 1 + EClass.rnd(2);
            Thing guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_thief").SetNum(guilpoNum);
            DropReward(guilpo);
            ThiefGuildZone.ModInfluence(1);
        }
    }
}
