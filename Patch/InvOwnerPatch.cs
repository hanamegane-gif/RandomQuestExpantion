using HarmonyLib;
using RandomQuestExpantion.Guilpo;

namespace RandomQuestExpantion.Patch
{
    // 取引画面の通貨残量を表示
    // 「売る」表記を独自通貨の名前に対応させる
    // 独自通貨での売買を可能にする
    // 売買画面での独自通貨のアイコンを表示させる
    // 独自通貨を使用する場合の価格を取得する
    [HarmonyPatch]
    class InvOwnerPatch
    {
        [HarmonyPatch(typeof(InvOwner), nameof(InvOwner.GetPrice)), HarmonyPrefix]
        public static bool GetPricePatch(ref int __result, InvOwner __instance, Thing t, CurrencyType currency, int num, bool sell)
        {
            var ownerTrait = __instance.owner.trait;
            var traderTrait = InvOwner.Trader.owner.trait;
            TraitGuilpoVender trait = (ownerTrait is TraitGuilpoVender) ? (TraitGuilpoVender)ownerTrait :
                                      (traderTrait is TraitGuilpoVender) ? (TraitGuilpoVender)traderTrait : null;
            if (trait == null)
            {
                return true;
            }

            __result = PriceSetting.GetPrice(t, num);
            return false;
        }

        [HarmonyPatch(typeof(InvOwner), nameof(InvOwner.BuildUICurrency)), HarmonyPrefix]
        public static bool BuildUICurrencyPatch(InvOwner __instance, UICurrency uiCurrency, bool canReroll)
        {
            var ownerTrait = __instance.owner.trait;
            if (!(__instance.owner.trait is TraitGuilpoVender))
            {
                return true;
            }

            uiCurrency.SetActive(true);
            uiCurrency.target = __instance.owner;
            uiCurrency.Build();
            uiCurrency.Add(
                EMono.sources.cards.map["ecopo"].GetSprite(),
                "byakko_mod_guilpo_unit",
                () => EMono.pc.GetCurrency(((TraitGuilpoVender)ownerTrait).CurrencyID).ToString("#,0") ?? ""
            );
            uiCurrency.layout.RebuildLayout();

            return false;
        }


        [HarmonyPatch(typeof(InvOwner), nameof(InvOwner.GetTextDetail)), HarmonyPrefix]
        public static bool GetTooltipTextPatch(ref string __result, InvOwner __instance, Thing t, CurrencyType currency, int num, bool sell)
        {
            var ownerTrait = __instance.owner.trait;
            var traderTrait = InvOwner.Trader.owner.trait;
            TraitGuilpoVender trait = (ownerTrait is TraitGuilpoVender) ? (TraitGuilpoVender)ownerTrait :
                                      (traderTrait is TraitGuilpoVender) ? (TraitGuilpoVender)traderTrait : null;
            if (trait == null)
            {
                return true;
            }

            int price = __instance.GetPrice(t, currency, num, sell);
            string currencyName = EClass.sources.things.map[trait.CurrencyID].GetName();
            string ref2 = ((price == 0) ? "" : "invInteraction3".lang(price.ToFormat() ?? "", currencyName));
            string text = "invInteraction1".lang(num.ToString() ?? "", ref2, (sell ? "invSell" : "invBuy").lang());
            if (!sell && EClass.pc.GetCurrency(trait.CurrencyID) < price)
            {
                text = text.TagColor(FontColor.Bad, SkinManager.DarkColors);
            }
            __result = text;

            return false;
        }

        // エコポで仮置きしていた通貨のidを置き換える
        [HarmonyPatch(typeof(InvOwner.Transaction), "get_IDCurrency"), HarmonyPrefix]
        internal static bool IDCurrencyPatch(ref string __result, InvOwner.Transaction __instance)
        {
            var ownerTrait = __instance.destInv.owner.trait;
            var traderTrait = InvOwner.Trader.owner.trait;
            TraitGuilpoVender trait = (ownerTrait is TraitGuilpoVender) ? (TraitGuilpoVender)ownerTrait :
                                      (traderTrait is TraitGuilpoVender) ? (TraitGuilpoVender)traderTrait : null;
            if (trait is TraitGuilpoVender)
            {
                __result = trait.CurrencyID;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(InvOwner), "get_IDCurrency"), HarmonyPrefix]
        internal static bool IDCurrencyPatch(ref string __result, InvOwner __instance)
        {
            var ownerTrait = __instance.owner.trait;
            var traderTrait = InvOwner.Trader.owner.trait;
            TraitGuilpoVender trait = (ownerTrait is TraitGuilpoVender) ? (TraitGuilpoVender)ownerTrait :
                                      (traderTrait is TraitGuilpoVender) ? (TraitGuilpoVender)traderTrait : null;

            if (trait is TraitGuilpoVender)
            {
                __result = trait.CurrencyID;
                return false;
            }
            return true;
        }

        // 独自通貨のアイコンはないのでエコポアイコンで代用する
        [HarmonyPatch(typeof(InvOwner), nameof(InvOwner.IDCostIcon)), HarmonyPrefix]
        internal static bool QuestCreatePatch(ref string __result, InvOwner __instance, ref Thing t)
        {
            var ownerTrait = __instance.owner.trait;
            var traderTrait = InvOwner.Trader.owner.trait;
            TraitGuilpoVender trait = (ownerTrait is TraitGuilpoVender) ? (TraitGuilpoVender)ownerTrait :
                                      (traderTrait is TraitGuilpoVender) ? (TraitGuilpoVender)traderTrait : null;
            if (trait is TraitGuilpoVender)
            {
                __result = "icon_ecopo";
                return false;
            }
            return true;
        }
    }
}
