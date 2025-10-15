using Newtonsoft.Json;

namespace RandomQuestExpantion.ModQuestTask
{
    class TaskSales : QuestTask
    {
        [JsonProperty]
        public int SoldAmount = 0;

        [JsonProperty]
        public int SoldAmountRequirements = 0;

        public override string RefDrama1 => SoldAmount.ToString() ?? "";

        public override string RefDrama2 => SoldAmountRequirements.ToString() ?? "";

        public override bool IsComplete()
        {
            return (SoldAmount >= SoldAmountRequirements);
        }

        public override void OnInit()
        {
            SoldAmountRequirements = 2000 + (owner.difficulty - 1) * 500;
        }

        public override string GetTextProgress()
        {
            return "byakko_mod_progress_sales".lang(RefDrama1, RefDrama2);
        }

        public virtual void OnSoldMerchandise(in Thing merchandise)
        {
            SoldAmount += merchandise.GetPrice(CurrencyType.Money, sell: true, PriceType.PlayerShop);
        }
    }
}
