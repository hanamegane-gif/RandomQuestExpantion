using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuests.QuestAttribute;

namespace RandomQuestExpantion.ModQuests.Common
{
    public class QuestCrimFactory : QuestHarvest, IHarvest
    {
        public override string IdZone => "instance_crim";

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventCrimFactory();
        }

        public override void OnInit()
        {
            int drugWeight = 100;
            int norma = 30 + EClass.rndHalf((difficulty - 1) * 3 + 1);

            destWeight = drugWeight * norma;
        }

        public override string GetTextProgress()
        {
            return "byakko_mod_progress_crim_produce".lang(Lang._weight(weightDelivered), Lang._weight(destWeight));
        }

        public override void OnBeforeComplete()
        {
            // 持ち込み対策がめんどくさいのでボーナスは出さない
        }

        public bool IsQuestItem(in Thing t)
        {
            if (t.id == "drug_crim" && !t.IsImportant)
            {
                return true;
            }

            return false;
        }
    }
}
