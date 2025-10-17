using System;
using System.Collections.Generic;
using System.Linq;

class TraitFGGuilpoVender : TraitGuilpoVender
{
    public override string CurrencyID => "MOD_byakko_RQX_guilpo_fighter";

    internal List<string> WeaponTypeList { get; } = new List<string>
    {
        "sword",
        "dagger",
        "blunt",
        "axe",
        "polearm",
        "scythe",
        "martial",
    };

    internal override void GenerateMerchantStock(Thing merchantChest)
    {
        int generateLv = CalcGenerateLv();

        foreach (var skill in WeaponTypeList)
        {
            string weaponId = (EClass.rnd(4) == 0) ? null : PickRandomWeaponID(skill);
            if (weaponId != null)
            {
                AddStockByThing(merchantChest, GenerateEquipment(weaponId, generateLv));
            }
        }

        int runeStock = 2 + EClass.rnd(3);
        for (int i = 0; i < runeStock; i++)
        {
            AddStockByThing(merchantChest, GenerateRune(generateLv));
        }

        // 8280: 軽量化(羽巻)
        AddStockById(merchantChest, "scroll_random", stockNum: 100, bless: BlessedState.Blessed, fixedRefVal: SPELL.SpLighten);
        AddStockById(merchantChest, "scroll_random", stockNum: 100, bless: BlessedState.Cursed, fixedRefVal: SPELL.SpLighten);
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

        if (filterRoll < 25)
        {
            // 武器エンチャ系
            rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub != "eleAttack";
        }
        else if (filterRoll < 50)
        {
            // 属性追加
            rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub == "eleAttack";
        }
        else if (filterRoll < 75)
        {
            // 特攻
            rareEnchantFilter = row => row.encSlot == "weapon" && row.alias.Contains("bane_");
        }
        else if (filterRoll < 87)
        {
            // 連撃慧眼ヴォーパル突撃者
            rareEnchantFilter = row =>
            (
                row.id == ENC.mod_flurry ||
                row.id == ENC.encHit ||
                row.id == SKILL.vopal ||
                row.id == ENC.rusher
            );
        }
        else
        {
            // 逆襲全特攻盾の暴君パリィ
            rareEnchantFilter = row =>
            (
                row.id == ENC.mod_frustration ||
                row.id == ENC.bane_all ||
                row.id == ENC.basher ||
                row.id == ENC.parry
            );
        }

        return rareEnchantFilter;
    }

    internal override Func<SourceElement.Row, bool> GetWeaponEnchantFilter()
    {
        Func<SourceElement.Row, bool> rareEnchantFilter = row => true;
        int filterRoll = EClass.rnd(100);

        if (filterRoll < 25)
        {
            // 武器エンチャ系
            rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub != "eleAttack";
        }
        else if (filterRoll < 40)
        {
            // 属性追加
            rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub == "eleAttack";
        }
        else if (filterRoll < 65)
        {
            // 特攻
            rareEnchantFilter = row => row.encSlot == "weapon" && row.alias.Contains("bane_");
        }
        else if (filterRoll < 83)
        {
            // 連撃慧眼ヴォーパル突撃者
            rareEnchantFilter = row =>
            (
                row.id == ENC.mod_flurry ||
                row.id == ENC.encHit ||
                row.id == SKILL.vopal ||
                row.id == ENC.rusher
            );
        }
        else
        {
            // 逆襲全特攻盾の暴君パリィ
            rareEnchantFilter = row =>
            (
                row.id == ENC.mod_frustration ||
                row.id == ENC.bane_all ||
                row.id == ENC.basher ||
                row.id == ENC.parry
            );
        }

        return rareEnchantFilter;
    }
}