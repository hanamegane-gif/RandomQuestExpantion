using Newtonsoft.Json;

namespace RandomQuestExpantion.ModQuestTask
{
    class TaskShipping : QuestTask
    {
        [JsonProperty]
        public int ShippedAmount = 0;

        [JsonProperty]
        public int ShippedAmountRequirements = 0;

        public override string RefDrama1 => ShippedAmount.ToString() ?? "";

        public override string RefDrama2 => ShippedAmountRequirements.ToString() ?? "";

        public override bool IsComplete()
        {
            return (ShippedAmount >= ShippedAmountRequirements);
        }

        public override void OnInit()
        {
            ShippedAmountRequirements = 3000 + (owner.difficulty - 1) * 500;
        }

        public override string GetTextProgress()
        {
            return "byakko_mod_progress_shipping".lang(RefDrama1, RefDrama2);
        }

        public virtual void OnShipped(int priceAmount)
        {
            ShippedAmount += priceAmount;
        }
    }
}
