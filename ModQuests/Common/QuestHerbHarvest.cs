using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuests.QuestAttribute;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.Common
{
    public class QuestHerbHarvest : QuestHarvest, IHarvest
    {
        public override string RefDrama2 => WeightText(destWeight);

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventHerbHarvest();
        }

        public override void OnInit()
        {
            int kusaWeight = 30;
            int norma = 15 + EClass.rndHalf((difficulty - 1) * 2 + 1);

            destWeight = (kusaWeight * norma / 100) * 100;
        }

        public override void OnBeforeComplete()
        {
            // 持ち込み対策がめんどくさいのでボーナスは出さない
        }

        public override string GetTextProgress()
        {
            return "progressHarvest".lang(WeightText(weightDelivered), WeightText(destWeight));
        }

        public bool IsQuestItem(in Thing t)
        {
            if (t.category.id == "herb" && !(t.IsImportant || t.isCrafted || t.encLV > 0))
            {
                return true;
            }

            return false;
        }
    }
}
