using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuests.QuestAttribute;

namespace RandomQuestExpantion.ModQuests.Common
{
    public class QuestOrganizingBook : QuestHarvest, IHarvest
    {
        public override string IdZone => "instance_study";

        public override string RewardSuffix => "";

        public override string RefDrama2 => (destWeight / 20).ToString();

        public override int DangerLv => 6;

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventOrganizingBook();
        }

        public override void OnInit()
        {
            int bookWeight = 20;
            int norma = 8 + difficulty / 2 + EClass.rnd(difficulty + 1) / 2;

            destWeight = bookWeight * norma;
        }

        public override void OnBeforeComplete()
        {
            // クエスト内部で既に出ているのでボーナスは出さない
        }

        public override string GetTextProgress()
        {
            return "byakko_mod_progress_book_recover".lang((weightDelivered / 20).ToString(), (destWeight / 20).ToString());
        }

        public bool IsQuestItem(in Thing t)
        {
            if (t.IsImportant)
            { 
                return false;
            }

            if (t.id == "book_ancient" && (t.GetBool(115) || t.isOn))
            {
                return true;
            }
            else if(t.id == "372" && t.GetBool(115))
            {
                return true;
            }

            return false;
        }
    }
}
