using System.Collections.Generic;
using UnityEngine;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestHQFurniture : QuestHQCraft
    {
        internal override void SetTargetCategory()
        {
            idCat = PickTargetFurnitureCategory();
        }

        private string PickTargetFurnitureCategory()
        {
            List<SourceCategory.Row> furnitureCategories = new List<SourceCategory.Row>();

            // 鉢植え・カード・フィギアはクラフトできないので除外
            // 窓・装飾は特性を盛るのが困難なので除外
            // 特殊は欲しがる奴いないだろうということで除外
            // バニラで達成不可能なクエストは出さない
            foreach (SourceCategory.Row row in EClass.sources.categories.rows)
            {
                if (row.deliver > 0 && (row.IsChildOf("bed") || row.IsChildOf("chair") || row.IsChildOf("table")))
                {
                    furnitureCategories.Add(row);
                }
            }

            return furnitureCategories.RandomItem().id;
        }

        internal override void SetQualityRequirement()
        {
            // 名声だけを使って算出する、依頼難易度はあくまで期限の短さからくるものとする
            // 10の倍数でないと表記上と内部数値の違いから発生する齟齬が出る
            QualityLvRequirement = Mathf.Clamp((EClass.pc.FameLv / 15) * 10, 20, 50);
        }

        internal override void SetAttributeRequirement()
        {
            // 癒し(750),珍しさ(751),見た目(752)から出題する
            // 特性は十分な値を確保しやすいためスケーリングしない
            ElementIdRequirement = new int[] { 750, 751, 752 }.RandomItem();
            ElementLvRequirement = 50;
        }
    }
}
