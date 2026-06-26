using RandomQuestExpantion.Guilpo;
using System;
using System.Linq;
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

        // chanceによる抽選は残しつつレアエンチャは出やすくする
        Func<Func<SourceElement.Row, bool>> rareEnchantFilter;
        if (generatedGear.category.IsChildOf("melee"))
        {
            rareEnchantFilter = GetWeaponEnchantFilter;
        }
        else
        {
            rareEnchantFilter = GetArmorEnchantFilter;
        }

        generatedGear = AddBonusRareEnchants(generatedGear, 2, generateLv, rareEnchantFilter);
        generatedGear.Identify(show: false, idtSource: IDTSource.SuperiorIdentify);

        return generatedGear;
    }
    internal virtual Thing GenerateRangedWeapon(string idThing, int generateLv)
    {
        var gearRarity = (EClass.rnd(5) == 0) ? Rarity.Mythical : Rarity.Legendary;
        var bp = new CardBlueprint { rarity = gearRarity, blesstedState = BlessedState.Normal };
        CardBlueprint.Set(bp);

        // スロット数やmodなどはバニラのルールに従う
        var generatedGear = ThingGen.Create(idThing, lv: generateLv);
        generatedGear.Identify(show: false, idtSource: IDTSource.SuperiorIdentify);

        return generatedGear;
    }

    internal virtual Thing GenerateRune(int generateLv, string forceId = "", bool reverse = false)
    {
        var rune = ThingGen.Create("rune");
        SourceElement.Row enchant;
        if (!forceId.IsEmpty())
        {
            enchant = EClass.sources.elements.rows.Where(r => r.alias == forceId).FirstOrDefault();
        }
        else
        {
            enchant = PickBonusEnchant(rune, generateLv, GetRuneEnchantFilter());
        }

        if (enchant == null)
        {
            return rune;
        }
        int enchantStrength = CalcEnchantMagnitude(enchant, generateLv);

        rune.refVal = enchant.id;
        rune.encLV = enchantStrength * (reverse ? -1 : 1);

        return rune;
    }

    internal void AddStockById(Thing merchantChest, string id, int stockNum = -1, BlessedState bless = BlessedState.Normal, int charges = -1, int generateLv = 20, int fixedRefVal = -1, int idMat = -1, int idSkin = 0, int lv = -1)
    {
        var bp = new CardBlueprint { rarity = Rarity.Normal, blesstedState = bless };
        CardBlueprint.Set(bp);

        var createdThing = ThingGen.Create(id, idMat, generateLv);

        if (fixedRefVal != -1)
        {
            createdThing.refVal = fixedRefVal;
        }
        else if (createdThing.refVal != 0)
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

        createdThing.SetNum((stockNum != -1) ? stockNum : StackSetting.GetStackNum(createdThing));

        if (createdThing.trait.HasCharges)
        {
            createdThing.SetCharge((charges != -1) ? charges : StackSetting.GetChargeNum(createdThing));
        }

        createdThing.idSkin = ((idSkin == -1) ? EClass.rnd(createdThing.source.skins.Length + 1) : idSkin);

        AddStockByThing(merchantChest, createdThing);
    }

    internal void AddStockByThing(Thing merchantChest, Thing stock)
    {
        stock.Identify(show: false, idtSource: IDTSource.SuperiorIdentify);
        merchantChest.AddThing(stock);
    }

    internal virtual Func<SourceElement.Row, bool> GetArmorEnchantFilter()
    {
        Func<SourceElement.Row, bool> rareEnchantFilter = row => true;
        int filterRoll = EClass.rnd(100);
        int chanceSum = 0;

        if (filterRoll < (chanceSum += 12))
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
        else if (filterRoll < (chanceSum += 12))
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
        else if (filterRoll < (chanceSum += 12))
        {
            // 戦闘系
            rareEnchantFilter = row => row.categorySub == "combat";
        }
        else if (filterRoll < (chanceSum += 12))
        {
            // 耐性
            rareEnchantFilter = row => row.type == "Resistance";
        }
        else if (filterRoll < (chanceSum += 12))
        {
            // 生産系
            rareEnchantFilter = row => row.categorySub == "craft" || row.categorySub == "labor";
        }
        else if (filterRoll < (chanceSum += 8))
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
        else if (filterRoll < (chanceSum += 8))
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
        int chanceSum = 0;

        if (filterRoll < (chanceSum += 12))
        {
            // 武器エンチャ系
            rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub != "eleAttack";
        }
        else if (filterRoll < (chanceSum += 12))
        {
            // 属性追加
            rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub == "eleAttack";
        }
        else if (filterRoll < (chanceSum += 12))
        {
            // 特攻
            rareEnchantFilter = row => row.encSlot == "weapon" && row.alias.Contains("bane_");
        }
        else if (filterRoll < (chanceSum += 12))
        {
            // 耐性
            rareEnchantFilter = row => row.type == "Resistance";
        }
        else if (filterRoll < (chanceSum += 12))
        {
            // 生産系
            rareEnchantFilter = row => row.categorySub == "craft" || row.categorySub == "labor";
        }
        else if (filterRoll < (chanceSum += 8))
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
        else if (filterRoll < (chanceSum += 8))
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
        int chanceSum = 0;

        if (filterRoll < (chanceSum += 12))
        {
            // 武器エンチャ系
            rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub != "eleAttack";
        }
        else if (filterRoll < (chanceSum += 12))
        {
            // 属性追加
            rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub == "eleAttack";
        }
        else if (filterRoll < (chanceSum += 12))
        {
            // 特攻
            rareEnchantFilter = row => row.encSlot == "weapon" && row.alias.Contains("bane_");
        }
        else if (filterRoll < (chanceSum += 12))
        {
            // 耐性
            rareEnchantFilter = row => row.type == "Resistance";
        }
        else if (filterRoll < (chanceSum += 12))
        {
            // 生産系
            rareEnchantFilter = row => row.categorySub == "craft" || row.categorySub == "labor";
        }
        else if (filterRoll < (chanceSum += 8))
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
        else if (filterRoll < (chanceSum += 8))
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

    internal static int CalcGenerateLv()
    {
        // 現状の進行度(ボスのLv)より少し弱い程度
        int progress = Mathf.Max(EClass.pc.FameLv, EClass.player.stats.deepest, 15);
        int genLv = (int)(Math.Min(progress * 4L / 3L, Int32.MaxValue));
        return Mathf.Max(genLv, 20);
    }
}
