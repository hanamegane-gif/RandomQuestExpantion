using System;
class TraitMGGuilpoVender : TraitGuilpoVender
{
    public override string CurrencyID => "MOD_byakko_RQX_guilpo_merchant";

    internal override void GenerateMerchantStock(Thing merchantChest)
    {
        int generateLv = CalcGenerateLv();


        int runeStock = 2 + EClass.rnd(3);
        for (int i = 0; i < runeStock; i++)
        {
            AddStockByThing(merchantChest, GenerateRune(generateLv));
        }

        int hammerStock = 1 + EClass.rnd(3);
        for (int i = 0; i < runeStock; i++)
        {
            SourceMaterial.Row randomMaterial = MATERIAL.GetRandomMaterial(80 + 10, tryLevelMatTier: true);
            AddStockById(merchantChest, "mathammer", stockNum: 1, idMat: randomMaterial.id);
        }

        // 8280: 軽量化(羽巻)
        AddStockById(merchantChest, "scroll_random", stockNum: 10, fixedRefVal: 8288);
        AddStockById(merchantChest, "rp_food", stockNum: 20);
        AddStockById(merchantChest, "medal", stockNum: 10);
        AddStockById(merchantChest, "plat", stockNum: 500);
        AddStockById(merchantChest, "bill_tax", stockNum: 3);
        AddStockById(merchantChest, "crystal_earth", stockNum: 20);
        AddStockById(merchantChest, "crystal_sun", stockNum: 20);
        AddStockById(merchantChest, "crystal_mana", stockNum: 20);
    }

    internal override Func<SourceElement.Row, bool> GetRuneEnchantFilter()
    {
        Func<SourceElement.Row, bool> rareEnchantFilter = row => true;
        int filterRoll = EClass.rnd(100);

        if (filterRoll < 20)
        {
            // 生産系
            rareEnchantFilter = row => row.categorySub == "craft" || row.categorySub == "labor";
        }
        else if (filterRoll < 53)
        {
            // 耐性
            rareEnchantFilter = row => row.type == "Resistance";
        }
        else
        {
            // 魅力交渉投資旅歩き
            rareEnchantFilter = row =>
            (
                row.id == SKILL.CHA ||
                row.id == SKILL.negotiation ||
                row.id == SKILL.investing ||
                row.id == SKILL.travel
            );
        }

        return rareEnchantFilter;
    }
}
