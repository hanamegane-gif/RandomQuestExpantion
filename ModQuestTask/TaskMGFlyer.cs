namespace RandomQuestExpantion.ModQuestTask
{
    class TaskMGFlyer : QuestTaskFlyer
    {
        public override void OnInit()
        {
            numRequired = 10 + (owner.difficulty - 1) * 5;
        }
    }
}
