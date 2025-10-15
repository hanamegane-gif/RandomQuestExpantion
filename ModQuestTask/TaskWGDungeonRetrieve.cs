using System;

namespace RandomQuestExpantion.ModQuestTask
{
    class TaskWGDungeonRetrieve : TaskDungeonRetrieve
    {
        internal override int CalcBonusMoney(in Chara boss)
        {
            return 0;
        }

        internal override Func<SourceElement.Row, bool> GetArmorEnchantFilter()
        {
            // 魔術師ギルドなので魔法系を出やすくする
            Func<SourceElement.Row, bool> rareEnchantFilter = row => true;
            int filterRoll = EClass.rnd(100);

            if (filterRoll < 12)
            {
                // 肉体系主能力
                rareEnchantFilter = row => row.category == "attribute" &&
                (
                    row.id == SKILL.STR ||
                    row.id == SKILL.END ||
                    row.id == SKILL.DEX ||
                    row.id == SKILL.CHA
                );
            }
            else if (filterRoll < 24)
            {
                // 精神系主能力
                rareEnchantFilter = row => row.category == "attribute" &&
                (
                    row.id == SKILL.PER ||
                    row.id == SKILL.LER ||
                    row.id == SKILL.WIL ||
                    row.id == SKILL.MAG
                );
            }
            else if (filterRoll < 32)
            {
                // 戦闘系
                rareEnchantFilter = row => row.categorySub == "combat";
            }
            else if (filterRoll < 44)
            {
                // 耐性
                rareEnchantFilter = row => row.type == "Resistance";
            }
            else if (filterRoll < 56)
            {
                // 生産系
                rareEnchantFilter = row => row.categorySub == "craft" || row.categorySub == "labor";
            }
            else if (filterRoll < 64)
            {
                // 魔法強化慧眼突撃者パリィ不屈
                rareEnchantFilter = row =>
                (
                    row.id == ENC.encSpell ||
                    row.id == ENC.encHit ||
                    row.id == ENC.rusher ||
                    row.id == ENC.parry ||
                    row.id == ENC.guts
                );
            }
            else if (filterRoll < 72)
            {
                // 魔法強化信仰見切り盾の暴君射撃防御
                rareEnchantFilter = row =>
                (
                    row.id == ENC.encSpell ||
                    row.id == SKILL.faith ||
                    row.id == SKILL.evasionPlus ||
                    row.id == ENC.basher ||
                    row.id == ENC.defense_range
                );
            }
            else if (filterRoll < 84)
            {
                // 魔法強化魔力意志魔力の限界マナ運用
                rareEnchantFilter = row =>
                (
                    row.id == ENC.encSpell ||
                    row.id == SKILL.WIL ||
                    row.id == SKILL.MAG ||
                    row.id == SKILL.manaCapacity ||
                    row.id == ENC.optimizeMana
                );
            }

            return rareEnchantFilter;
        }

        internal override Func<SourceElement.Row, bool> GetWeaponEnchantFilter()
        {
            // 魔術師ギルドなので魔法系を出やすくする
            Func<SourceElement.Row, bool> rareEnchantFilter = row => true;
            int filterRoll = EClass.rnd(100);

            if (filterRoll < 8)
            {
                // 武器エンチャ系
                rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub != "eleAttack";
            }
            else if (filterRoll < 20)
            {
                // 属性追加
                rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub == "eleAttack";
            }
            else if (filterRoll < 28)
            {
                // 特攻
                rareEnchantFilter = row => row.encSlot == "weapon" && row.alias.Contains("bane_");
            }
            else if (filterRoll < 40)
            {
                // 耐性
                rareEnchantFilter = row => row.type == "Resistance";
            }
            else if (filterRoll < 48)
            {
                // 生産系
                rareEnchantFilter = row => row.categorySub == "craft" || row.categorySub == "labor";
            }
            else if (filterRoll < 56)
            {
                // 連撃慧眼ヴォーパル突撃者パリィ不屈
                rareEnchantFilter = row => row.category == "attribute" &&
                (
                    row.id == ENC.mod_flurry ||
                    row.id == ENC.encHit ||
                    row.id == SKILL.vopal ||
                    row.id == ENC.rusher ||
                    row.id == ENC.guts
                );
            }
            else if (filterRoll < 68)
            {
                // 逆襲魔法強化全特攻盾の暴君射撃防御
                rareEnchantFilter = row => row.category == "attribute" &&
                (
                    row.id == ENC.mod_frustration ||
                    row.id == ENC.encSpell ||
                    row.id == ENC.bane_all ||
                    row.id == ENC.basher ||
                    row.id == ENC.defense_range
                );
            }
            else if (filterRoll < 80)
            {
                // 魔法強化魔力意志魔力の限界マナ運用
                rareEnchantFilter = row =>
                (
                    row.id == ENC.encSpell ||
                    row.id == SKILL.WIL ||
                    row.id == SKILL.MAG ||
                    row.id == SKILL.manaCapacity ||
                    row.id == ENC.optimizeMana
                );
            }

            return rareEnchantFilter;
        }
    }
}
