using Newtonsoft.Json;
using UnityEngine;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestHQCraft : QuestSupplyCat
    {
        // にゃー、にゃあー
        public override string RefDrama1 => Cat.GetName();

        public override string RefDrama2 => GetElementText(EClass.sources.elements.GetRow(2.ToString()), QualityLvRequirement);

        public override string RefDrama3 => GetElementText(EClass.sources.elements.GetRow(ElementIdRequirement.ToString()), QualityLvRequirement);

        [JsonProperty]
        public int QualityLvRequirement = 0;

        [JsonProperty]
        public int ElementIdRequirement = 0;

        [JsonProperty]
        public int ElementLvRequirement = 0;

        public override void SetIdThing()
        {
            SetQualityRequirement();
            SetAttributeRequirement();
            SetTargetCategory();
        }

        public override string GetTextProgress()
        {
            string @ref = ((GetDestThing() != null) ? "supplyInInv".lang().TagColor(FontColor.Good) : "supplyNotInInv".lang());

            return "byakko_mod_progress_HQ_craft".lang(RefDrama1 + Lang.space + TextExtra2.IsEmpty(""), RefDrama2 + RefDrama3, @ref);
        }

        public override bool IsDestThing(Thing t)
        {
            if (t.parentCard != null && !t.parentCard.trait.CanUseContent)
            {
                return false;
            }

            if (t.c_isImportant || t.isEquipped)
            {
                return false;
            }

            if (t.category.IsChildOf(idCat) && IsMeetQualityLvRequirement(t) & IsMeetAttributeLvRequirement(t))
            {
                return t.things.Count == 0;
            }

            return false;
        }

        public override int GetBonus(Thing t)
        {
            return t.GetPrice(CurrencyType.Money, sell: true, PriceType.Shipping, EClass.pc) * 3 * (QualityLvRequirement / 10) * (t.Evalue(ELEMENT.quality) / 10);
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2) + curve(bonusMoney / 100, 10, 10, 60);
        }

        internal virtual void SetTargetCategory()
        {
        }

        internal virtual void SetQualityRequirement()
        {
        }

        internal virtual void SetAttributeRequirement()
        {
        }

        internal string GetElementText(in SourceElement.Row row, int Lv)
        {
            string[] textArray = row.GetTextArray("textAlt");
            int textIndex = Mathf.Clamp(Lv / 10 + 1, (Lv < 0 || textArray.Length <= 2) ? 1 : 2, textArray.Length - 1);
            string text = "altEnc".lang("", textArray[textIndex], "");

            return text;
        }

        internal bool IsMeetQualityLvRequirement(in Thing t)
        {
            return t.Evalue(ELEMENT.quality) >= QualityLvRequirement;
        }

        internal bool IsMeetAttributeLvRequirement(in Thing t)
        {
            return t.Evalue(ElementIdRequirement) >= ElementLvRequirement;
        }
    }
}
