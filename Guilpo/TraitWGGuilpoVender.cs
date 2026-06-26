using System;
using System.Linq;
class TraitWGGuilpoVender : TraitGuilpoVender
{
    public override string CurrencyID => "MOD_byakko_RQX_guilpo_mage";

    internal override void GenerateMerchantStock(Thing merchantChest)
    {
        int generateLv = CalcGenerateLv();

        int staffStock = 2;
        for (int i = 0; i < staffStock; i++)
        {
            AddStockByThing(merchantChest, GenerateEquipment(PickRandomWeaponID("staff"), generateLv));
        }

        int runeStock = 2 + EClass.rnd(3);
        for (int i = 0; i < runeStock; i++)
        {
            AddStockByThing(merchantChest, GenerateRune(generateLv));
        }

        int rodStock = 2 + EClass.rnd(3);
        for (int i = 0; i < runeStock; i++)
        {
            AddStockById(merchantChest, "rod");
        }

        int bookStock = 3 + EClass.rnd(2);
        if (EClass.rnd(1000) == 0)
        {
            AddStockById(merchantChest, "372", charges: 1, fixedRefVal: SPELL.SpWish);
            bookStock--;
        }
        if (EClass.rnd(200) == 0)
        {
            AddStockById(merchantChest, "372", charges: 1, fixedRefVal: SPELL.SpHealJure);
            bookStock--;
        }
        if (EClass.rnd(200) == 0)
        {
            AddStockById(merchantChest, "372", charges: 1, fixedRefVal: SPELL.SpRebirth);
            bookStock--;
        }
        for (int i = 0; i < bookStock; i++)
        {
            AddStockById(merchantChest, "372", generateLv: 200);
        }

        AddStockById(merchantChest, "scroll_random", fixedRefVal: SPELL.SpLighten);
    }

    private string PickRandomWeaponID(string weaponSkill)
    {
        var candidateList = EClass.sources.things.rows.Where(r => r.category == weaponSkill && r.chance != 0).ToList();
        if (!candidateList.Any())
        {
            return null;
        }

        return candidateList.RandomItemWeighted(row => row.chance).id;
    }

    internal override Func<SourceElement.Row, bool> GetRuneEnchantFilter()
    {
        Func<SourceElement.Row, bool> rareEnchantFilter = row => true;
        int filterRoll = EClass.rnd(100);
        int chanceSum = 0;

        if (filterRoll < (chanceSum += 25))
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
        else if (filterRoll < (chanceSum += 25))
        {
            // 属性追加
            rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub == "eleAttack";
        }
        else if (filterRoll < (chanceSum += 25))
        {
            // 魔法強化詠唱魔力制御瞑想
            rareEnchantFilter = row =>
            (
                row.id == ENC.encSpell ||
                row.id == SKILL.casting ||
                row.id == SKILL.controlmana ||
                row.id == SKILL.meditation
            );
        }
        else
        {
            // マナ軽減魔力の限界暗記魔道具
            rareEnchantFilter = row =>
            (
                row.id == ENC.optimizeMana ||
                row.id == SKILL.manaCapacity ||
                row.id == SKILL.memorization ||
                row.id == SKILL.magicDevice
            );
        }


        return rareEnchantFilter;
    }

    internal override Func<SourceElement.Row, bool> GetWeaponEnchantFilter()
    {
        Func<SourceElement.Row, bool> rareEnchantFilter = row => true;
        int filterRoll = EClass.rnd(100);
        int chanceSum = 0;

        if (filterRoll < (chanceSum += 25))
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
        else if (filterRoll < (chanceSum += 25))
        {
            // 属性追加
            rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub == "eleAttack";
        }
        else if (filterRoll < (chanceSum += 25))
        {
            // 魔法強化詠唱魔力制御瞑想
            rareEnchantFilter = row =>
            (
                row.id == ENC.encSpell ||
                row.id == SKILL.casting ||
                row.id == SKILL.controlmana ||
                row.id == SKILL.meditation
            );
        }
        else
        {
            // マナ軽減魔力の限界暗記魔道具
            rareEnchantFilter = row =>
            (
                row.id == ENC.optimizeMana ||
                row.id == SKILL.manaCapacity ||
                row.id == SKILL.memorization ||
                row.id == SKILL.magicDevice
            );
        }

        return rareEnchantFilter;
    }
}