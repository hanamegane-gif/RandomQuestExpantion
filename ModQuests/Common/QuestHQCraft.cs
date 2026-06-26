using Newtonsoft.Json;
using RandomQuestExpantion.ModQuests.QuestAttribute;
using UnityEngine;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.Common
{
    public class QuestHQCraft : QuestSupplyCat, IExtendDeadline
    {
        // にゃー、にゃあー
        public override string RefDrama1 => Cat.GetName();

        public override string RefDrama2 => GetElementText(EClass.sources.elements.GetRow(2.ToString()), QualityLvRequirement);

        public override string RefDrama3 => GetElementText(EClass.sources.elements.GetRow(ElementIdRequirement.ToString()), ElementLvRequirement);

        public virtual int DaysExtraDeadline => 3 + 3 * (7 - Mathf.Clamp(this.difficulty, 1, 7)) / 2;

        public override int BaseMoney => source.money + (QualityLvRequirement / 10) * (QualityLvRequirement / 15) * 300;

        [JsonProperty]
        public int QualityLvRequirement = 0;

        [JsonProperty]
        public int ElementIdRequirement = 0;

        [JsonProperty]
        public int ElementLvRequirement = 0;

        public override void OnStart()
        {
            base.OnStart();
            OnStartExtendDeadline();
        }

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
            return t.GetPrice(CurrencyType.Money, sell: true, PriceType.Shipping, EClass.pc) * 3 * (QualityLvRequirement / 10) * ((t.Evalue(ELEMENT.quality) - QualityLvRequirement) / 15);
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2) + (QualityLvRequirement * 4 / 30);
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

        public Quest OnStartExtendDeadline()
        {
            deadline += DaysExtraDeadline * Date.DayToken;
            return this;
        }

        public string GetAltTextDeadline()
        {
            return AltTextDeadline((Hours >= 0) ? Hours : 0, DaysExtraDeadline);
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
