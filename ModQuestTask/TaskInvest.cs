using Newtonsoft.Json;

namespace RandomQuestExpantion.ModQuestTask
{
    class TaskInvest : QuestTask
    {
        [JsonProperty]
        public int InvestedTimes = 0;

        [JsonProperty]
        public int InvestRequirements = 0;

        public override string RefDrama1 => InvestedTimes.ToString() ?? "";

        public override string RefDrama2 => InvestRequirements.ToString() ?? "";

        public override bool IsComplete()
        {
            return InvestedTimes >= InvestRequirements;
        }

        public override void OnInit()
        {
            InvestRequirements = 4 + (owner.difficulty - 1) * 2;
        }

        public override string GetTextProgress()
        {
            return "byakko_mod_progress_invest".lang(RefDrama1, RefDrama2);
        }

        public virtual void OnInvest()
        {
            InvestedTimes++;
        }
    }
}
