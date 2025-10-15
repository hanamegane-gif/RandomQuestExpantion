using HarmonyLib;
using UnityEngine;

namespace RandomQuestExpantion.Patch
{
    // ギルポ交換所の在庫を設定する
    [HarmonyPatch]
    class TraitPatch
    {
        [HarmonyPatch(typeof(Trait), nameof(Trait.OnBarter)), HarmonyPrefix]
        internal static bool GuilpoStockPatch(Trait __instance)
        {
            if (__instance is TraitGuilpoVender)
            {
                TryRestock((TraitGuilpoVender)__instance);
                return false;
            }

            return true;
        }

        internal static void TryRestock(TraitGuilpoVender t)
        {
            var ownerCard = t.owner;
            Thing merchantChest = ownerCard.things.Find("chest_merchant");
            if (merchantChest == null)
            {
                merchantChest = ThingGen.Create("chest_merchant");
                ownerCard.AddThing(merchantChest);
            }
            merchantChest.c_lockLv = 0;
            if (!EClass.world.date.IsExpired(ownerCard.c_dateStockExpire))
            {
                return;
            }
            ownerCard.c_dateStockExpire = EClass.world.date.GetRaw(24 * t.RestockDay);
            ownerCard.isRestocking = true;
            merchantChest.things.DestroyAll((Thing _t) => _t.GetInt(101) != 0);
            foreach (Thing thing8 in merchantChest.things)
            {
                thing8.invX = -1;
            }

            t.GenerateMerchantStock(merchantChest);

            if (merchantChest.things.Count <= merchantChest.things.GridSize)
            {
                return;
            }
            int num15 = merchantChest.things.width * 10;
            if (merchantChest.things.Count > num15)
            {
                int num16 = merchantChest.things.Count - num15;
                for (int num17 = 0; num17 < num16; num17++)
                {
                    merchantChest.things.LastItem().Destroy();
                }
            }
            merchantChest.things.ChangeSize(merchantChest.things.width, Mathf.Min(merchantChest.things.Count / merchantChest.things.width + 1, 10));
        }
    }
}
