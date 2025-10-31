using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static RandomQuestExpantion.General.General;

class TraitGuilpoVender : TraitVendingMachine
{
    public virtual string CurrencyID => "ecopo";

    public override string IDInvStyle => "default";

    public override ShopType ShopType => ShopType.Guild;

    public override bool CanBeDropped => false;

    public override bool CanStack => false;

    public override bool CanBeStolen => false;

    public override bool CanBeDestroyed => false;

    // エコポで仮置き
    public override CurrencyType CurrencyType => CurrencyType.Ecopo;

    public override PriceType PriceType => PriceType.Default;

    public override int CostRerollShop => 3;

    public override bool AllowSell => false;

    public override bool OnUse(Chara c)
    {
        OnBarter();
        EClass.ui.AddLayer(LayerInventory.CreateBuy(owner, this.CurrencyType, this.PriceType));
        return false;
    }

    internal virtual void GenerateMerchantStock(Thing merchantChest)
    {
        AddStockById(merchantChest, "record", 1);
    }

    internal virtual Thing GenerateEquipment(string idThing, int generateLv)
    {
        // 奇跡以上確定の場合のバニラの神器率は5%
        var gearRarity = (EClass.rnd(5) == 0) ? Rarity.Mythical : Rarity.Legendary;
        var bp = new CardBlueprint { rarity = gearRarity, blesstedState = BlessedState.Normal };
        CardBlueprint.Set(bp);

        var generatedGear = ThingGen.Create(idThing, lv: generateLv);

        // 素材エンチャが破壊に巻き込まれないようにするため一旦ダークマターにする
        var originalMaterial = generatedGear.material;
        generatedGear.ChangeMaterial("void");

        for (int i = 0; i < 2; i++)
        {
            RemoveEnchantRandomOne(generatedGear);
        }

        for (int i = 0; i < 2; i++)
        {
            var bonusEnchant = PickBonusEnchant(generatedGear, generateLv);
            if (bonusEnchant == null)
            {
                continue;
            }

            var bonusEnchantStrength = CalcEnchantStrength(bonusEnchant, generateLv);
            generatedGear.elements.ModBase(bonusEnchant.id, bonusEnchantStrength);
        }

        generatedGear.ChangeMaterial(originalMaterial);
        generatedGear.Identify(show: false, idtSource: IDTSource.SuperiorIdentify);

        return generatedGear;
    }
    internal virtual Thing GenerateRangedWeapon(string idThing, int generateLv)
    {
        // 奇跡以上確定の場合のバニラの神器率は5%
        // 遠隔武器の厳選は6s神器が出たらおしまいになるので確率を絞る
        var gearRarity = (EClass.rnd(20) == 0) ? Rarity.Mythical : Rarity.Legendary;
        var bp = new CardBlueprint { rarity = gearRarity, blesstedState = BlessedState.Normal };
        CardBlueprint.Set(bp);

        // スロット数やmodなどはバニラのルールに従う
        var generatedGear = ThingGen.Create(idThing, lv: generateLv);
        generatedGear.Identify(show: false, idtSource: IDTSource.SuperiorIdentify);

        return generatedGear;
    }

    internal virtual Thing GenerateRune(int generateLv, string forceId = "", bool reverse = false)
    {
        Thing rune = ThingGen.Create("rune");
        SourceElement.Row enchant;
        if (!forceId.IsEmpty())
        {
            enchant = EClass.sources.elements.rows.Where(r => r.alias == forceId).FirstOrDefault();
        }
        else
        {
            enchant = PickBonusEnchant(rune, generateLv);
        }

        if (enchant == null)
        {
            return rune;
        }
        var enchantStrength = CalcEnchantStrength(enchant, generateLv);

        rune.refVal = enchant.id;
        rune.encLV = enchantStrength * (reverse ? -1 : 1);

        return rune;
    }

    internal void AddStockById(Thing merchantChest, string id, int stockNum = -1, BlessedState bless = BlessedState.Normal, int charges = -1, int generateLv = 20, int fixedRefVal = -1, int idMat = -1, int idSkin = 0, int lv = -1)
    {
        var bp = new CardBlueprint { rarity = Rarity.Normal, blesstedState = bless };
        CardBlueprint.Set(bp);

        Thing createdThing = ThingGen.Create(id, idMat, generateLv);
        createdThing.SetNum((stockNum != -1) ? stockNum : CalcItemStack(createdThing));

        if (createdThing.trait.HasCharges)
        {
            createdThing.SetCharge((charges != -1) ? charges : CalcItemCharge(createdThing));
        }

        if (fixedRefVal != -1)
        {
            createdThing.refVal = fixedRefVal;
        }
        else if(createdThing.refVal != 0)
        {
            if (EClass.sources.elements.rows.Where(r => r.id == createdThing.refVal).First().tag.Contains("noShop"))
            {
                //noShopはレア枠にする
                if (EClass.rnd(5) != 0)
                {
                    return;
                }
            }
        }

        if (lv != -1)
        {
            createdThing.SetLv(lv);
        }

        if (lv != -1)
        {
            createdThing.SetLv(lv);
        }

        createdThing.idSkin = ((idSkin == -1) ? EClass.rnd(createdThing.source.skins.Length + 1) : idSkin);

        AddStockByThing(merchantChest, createdThing);
    }

    internal void AddStockByThing(Thing merchantChest, Thing stock)
    {
        stock.Identify(show: false, idtSource: IDTSource.SuperiorIdentify);
        merchantChest.AddThing(stock);
    }

    internal virtual SourceElement.Row PickBonusEnchant(in Thing generatedThing, int generateLv)
    {
        if (EClass.rnd(100) == 0)
        {
            return EClass.sources.elements.rows.Where(r => r.alias == "slot_rune").FirstOrDefault();
        }

        var candidateList = new List<SourceElement.Row>();
        var enchantType = (generatedThing.trait is TraitRune) ? "rune" :
                          (generatedThing.category.IsChildOf("melee") ? "melee" : "armor");
        var gearCategory = generatedThing.category;

        Func<SourceElement.Row, bool> applicableFilter = (enchantType == "rune") ? row => true : row => row.IsEncAppliable(gearCategory);
        // フラグ系エンチャがボーナスで付くのはかわいそうなので弾いておく
        Func<SourceElement.Row, bool> flagEnchantFilter = (enchantType == "rune") ? row => true : row => !row.tag.Contains("flag");

        // chanceによる抽選は残しつつレアエンチャは出やすくする
        Func<SourceElement.Row, bool> rareEnchantFilter;
        if (enchantType == "melee")
        {
            rareEnchantFilter = GetWeaponEnchantFilter();
        }
        else if (enchantType == "armor")
        {
            rareEnchantFilter = GetArmorEnchantFilter();
        }
        else
        {
            rareEnchantFilter = GetRuneEnchantFilter();
        }

        int sumChance = 0;

        foreach (var enchant in EClass.sources.elements.rows.Where(r => applicableFilter(r) && flagEnchantFilter(r) && rareEnchantFilter(r) && !r.tag.Contains("unused")))
        {
            if (enchant.LV < generateLv + 15)
            {
                candidateList.Add(enchant);
                sumChance += enchant.chance;
            }
        }

        if (sumChance == 0)
        {
            return null;
        }

        int enchantRoll = EClass.rnd(sumChance);
        int temp = 0;
        foreach (var enchant in candidateList)
        {
            temp += enchant.chance;
            if (enchantRoll < temp)
            {
                return enchant;
            }
        }

        return null;
    }

    internal virtual Func<SourceElement.Row, bool> GetArmorEnchantFilter()
    {
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
        else if (filterRoll < 36)
        {
            // 戦闘系
            rareEnchantFilter = row => row.categorySub == "combat";
        }
        else if (filterRoll < 48)
        {
            // 耐性
            rareEnchantFilter = row => row.type == "Resistance";
        }
        else if (filterRoll < 60)
        {
            // 生産系
            rareEnchantFilter = row => row.categorySub == "craft" || row.categorySub == "labor";
        }
        else if (filterRoll < 68)
        {
            // 慧眼反魔突撃者パリィ不屈
            rareEnchantFilter = row =>
            (
                row.id == ENC.encHit ||
                row.id == SKILL.antiMagic ||
                row.id == ENC.rusher ||
                row.id == ENC.parry ||
                row.id == ENC.guts
            );
        }
        else if (filterRoll < 76)
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

        return rareEnchantFilter;
    }

    internal virtual Func<SourceElement.Row, bool> GetRuneEnchantFilter()
    {
        Func<SourceElement.Row, bool> rareEnchantFilter = row => true;
        int filterRoll = EClass.rnd(100);

        if (filterRoll < 12)
        {
            // 武器エンチャ系
            rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub != "eleAttack";
        }
        else if (filterRoll < 24)
        {
            // 属性追加
            rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub == "eleAttack";
        }
        else if (filterRoll < 36)
        {
            // 特攻
            rareEnchantFilter = row => row.encSlot == "weapon" && row.alias.Contains("bane_");
        }
        else if (filterRoll < 48)
        {
            // 耐性
            rareEnchantFilter = row => row.type == "Resistance";
        }
        else if (filterRoll < 60)
        {
            // 生産系
            rareEnchantFilter = row => row.categorySub == "craft" || row.categorySub == "labor";
        }
        else if (filterRoll < 68)
        {
            // 連撃慧眼ヴォーパル突撃者パリィ不屈
            rareEnchantFilter = row =>
            (
                row.id == ENC.mod_flurry ||
                row.id == ENC.encHit ||
                row.id == SKILL.vopal ||
                row.id == ENC.rusher ||
                row.id == ENC.guts
            );
        }
        else if (filterRoll < 76)
        {
            // 逆襲魔法強化全特攻盾の暴君射撃防御
            rareEnchantFilter = row =>
            (
                row.id == ENC.mod_frustration ||
                row.id == ENC.encSpell ||
                row.id == ENC.bane_all ||
                row.id == ENC.basher ||
                row.id == ENC.defense_range
            );
        }

        return rareEnchantFilter;
    }

    internal virtual Func<SourceElement.Row, bool> GetWeaponEnchantFilter()
    {
        Func<SourceElement.Row, bool> rareEnchantFilter = row => true;
        int filterRoll = EClass.rnd(100);

        if (filterRoll < 12)
        {
            // 武器エンチャ系
            rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub != "eleAttack";
        }
        else if (filterRoll < 24)
        {
            // 属性追加
            rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub == "eleAttack";
        }
        else if (filterRoll < 36)
        {
            // 特攻
            rareEnchantFilter = row => row.encSlot == "weapon" && row.alias.Contains("bane_");
        }
        else if (filterRoll < 48)
        {
            // 耐性
            rareEnchantFilter = row => row.type == "Resistance";
        }
        else if (filterRoll < 60)
        {
            // 生産系
            rareEnchantFilter = row => row.categorySub == "craft" || row.categorySub == "labor";
        }
        else if (filterRoll < 68)
        {
            // 連撃慧眼ヴォーパル突撃者パリィ不屈
            rareEnchantFilter = row =>
            (
                row.id == ENC.mod_flurry ||
                row.id == ENC.encHit ||
                row.id == SKILL.vopal ||
                row.id == ENC.rusher ||
                row.id == ENC.guts
            );
        }
        else if (filterRoll < 76)
        {
            // 逆襲魔法強化全特攻盾の暴君射撃防御
            rareEnchantFilter = row =>
            (
                row.id == ENC.mod_frustration ||
                row.id == ENC.encSpell ||
                row.id == ENC.bane_all ||
                row.id == ENC.basher ||
                row.id == ENC.defense_range
            );
        }

        return rareEnchantFilter;
    }

    internal virtual int CalcItemStack(in Thing stackableThing)
    {
        var trait = stackableThing.trait;

        if (trait == null || !trait.CanStack)
        {
            return 1;
        }

        if (stackableThing.trait is TraitCurrencyMedal)
        {
            return 50;
        }

        if (stackableThing.trait is TraitCurrency)
        {
            return 500;
        }

        if (stackableThing.trait is TraitDrink || stackableThing.trait is TraitScroll)
        {
            return 999;
        }

        if (stackableThing.trait is TraitMaterialHammer)
        {
            return 3;
        }

        return 1;
    }

    internal virtual int CalcItemCharge(in Thing chargableThing)
    {
        var trait = chargableThing.trait;

        if (trait == null || !trait.HasCharges)
        {
            return 0;
        }

        if (trait is TraitRod)
        {
            return 30 + EClass.rnd(10);
        }

        if (trait is TraitSpellbook)
        {
            return 10;
        }
        return 1;
    }

    internal static int CalcGenerateLv()
    {
        // 現状の進行度(ボスのLv)より少し弱い程度
        int progress = Mathf.Max(EClass.pc.FameLv, EClass.player.stats.deepest, 15);
        int genLv = (int)(Math.Min(progress * 4L / 3L, Int32.MaxValue));
        return Mathf.Max(genLv, 20);
    }

    internal static int CalcEnchantStrength(in SourceElement.Row enchant, int generateLv)
    {
        if (enchant.alias == "meleeDistance")
        {
            return 1;
        }

        if (enchant.alias == "ActNeckHunt")
        {
            return 5 + EClass.rnd(11);
        }

        if (enchant.alias == "slot_rune")
        {
            return (EClass.rnd(4) == 0) ? 2 : 1;
        }

        int linear = 3 + Mathf.Min(generateLv / 10, 15);
        int curvy = (int)Math.Min((long)generateLv * enchant.encFactor / 100, Int32.MaxValue);

        int maxStrength = linear + (int)Mathf.Sqrt(curvy);

        int strength = (maxStrength * 7 / 10) + EClass.rnd(1 + maxStrength * 3 / 10);
        strength = (enchant.mtp + strength) / enchant.mtp;

        if (enchant.encFactor == 0 && strength > 25)
        {
            strength = 25;
        }

        return strength;
    }
}
