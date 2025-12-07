using RandomQuestExpantion.ModQuestEvent;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestCrimFactory : QuestHarvest
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

        public override void OnBeforeComplete()
        {
            // 持ち込み対策がめんどくさいのでボーナスは出さない
        }

        internal virtual bool IsQuestItem(in Thing t)
        {
            if (t.id == "drug_crim" && !t.IsImportant)
            {
                return true;
            }

            return false;
        }
    }
}
