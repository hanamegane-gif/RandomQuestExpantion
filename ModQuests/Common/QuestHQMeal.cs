using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestHQMeal : QuestHQCraft
    {
        internal override void SetTargetCategory()
        {
            List<SourceCategory.Row> list = EClass.sources.categories.rows.Where((SourceCategory.Row c) => c._parent == "meal" && c.id != "meal_lunch").ToList();
            idCat = list.RandomItem().id;
        }

        internal override void SetQualityRequirement()
        {
            // 名声だけを使って算出する、依頼難易度はあくまで期限の短さからくるものとする
            QualityLvRequirement = Mathf.Clamp((EClass.pc.FameLv / 10) * 10, 20, 50);
        }

        internal override void SetAttributeRequirement()
        {
            // 潜在は上げやすいので主能力から出題する
            // どの進行度でも主能力特性は十分な値を確保できるためスケーリングしない
            ElementIdRequirement = Element.List_MainAttributesMajor.RandomItem();
            ElementLvRequirement = 50;
        }
    }
}
