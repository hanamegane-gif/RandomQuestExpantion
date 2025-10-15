using System.Collections.Generic;
class TraitTGGuilpoVender : TraitGuilpoVender
{
    public override string CurrencyID => "MOD_byakko_RQX_guilpo_thief";

    internal List<string> NomalRuneIdList { get; } = new List<string>
    {
        "lockpicking",
        "stealing",
    };

    // 発動など基本的にネタ枠のルーン
    internal List<string> RareRuneIdList { get; } = new List<string>
    {
        "meleeDistance",
        "ActNeckHunt",
        "ActStealMoney",
        "SpTeleportShort",
        "SpGravity",
        "SpSpeedDown",
        "SpBerserk", // これは自分にかかる
        "SpTransmutePutit", // これは相手にかかる
        "ActDraw",
    };

    // マイナス生産ルーン
    internal List<string> CursedRuneIdList { get; } = new List<string>
    {
        "swimming",
        "travel",
        "carpentry",
        "blacksmith",
        "alchemy",
        "sculpture",
        "jewelry",
        "weaving",
        "handicraft",
        "cooking",
    };

    internal override void GenerateMerchantStock(Thing merchantChest)
    {
        int generateLv = CalcGenerateLv();
        int runeStock = 2 + EClass.rnd(3);
        for (int i = 0; i < runeStock; i++)
        {
            if (EClass.rnd(5) == 0)
            {
                AddStockByThing(merchantChest, GenerateRune(generateLv, forceId: RareRuneIdList.RandomItem()));
            }
            else if (EClass.rnd(2) == 0)
            {
                AddStockByThing(merchantChest, GenerateRune(generateLv, forceId: CursedRuneIdList.RandomItem(), reverse: true));
            }
            else
            {
                AddStockByThing(merchantChest, GenerateRune(generateLv, forceId: NomalRuneIdList.RandomItem()));
            }
        }

        if (EClass.rnd(1000) == 0)
        {
            AddStockById(merchantChest, "rod_wish", stockNum: 1, charges: 1);
        }
        else if (EClass.rnd(10) == 0)
        {
            AddStockById(merchantChest, "rod_wish", stockNum: 1, charges: 0);
        }

        if (EClass.rnd(4) == 0)
        {
            AddStockById(merchantChest, "1165", stockNum: 1);
        }

        // 8250: 武器強化 8251: 防具強化
        AddStockById(merchantChest, "338", stockNum: 100, bless: BlessedState.Cursed);
        AddStockById(merchantChest, "scroll_random", stockNum: 100, bless: BlessedState.Cursed, fixedRefVal: 8250);
        AddStockById(merchantChest, "scroll_random", stockNum: 100, bless: BlessedState.Cursed, fixedRefVal: 8251);
        AddStockById(merchantChest, "bucket", stockNum: 30, bless: BlessedState.Blessed);
        AddStockById(merchantChest, "bucket", stockNum: 30, bless: BlessedState.Cursed);
    }
}