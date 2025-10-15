namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestFlyer : QuestRandom
    {
        public override string RefDrama2 => Requirement.ToString();

        public override int KarmaOnFail => -3;

        public override string RewardSuffix => "";

        public override int RangeDeadLine => 20;

        public int Requirement => ((QuestTaskFlyer)task).numRequired;

        public override void OnInit()
        {
            SetTask(new QuestTaskFlyer());
        }

        public override void OnStart()
        {
            // なんとチラシ支給だ　喜べ
            Thing thing = ThingGen.Create("flyer");
            thing.Identify(show: false, IDTSource.SuperiorIdentify);
            thing.SetNum(Requirement * 3 / 2);
            Msg.Say("get_quest_item");
            EClass.pc.Pick(thing);
        }
    }
}
