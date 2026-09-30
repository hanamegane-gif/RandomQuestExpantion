using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomQuestExpantion.Guilpo
{
    class PriceSetting
    {
        internal static int GetPrice(in Thing thing, int num)
        {
            int price = 1;
            if (thing.IsWeapon || thing.IsEquipment)
            {
                price = GetGearPrice(thing);
            }
            else if (thing.id.Contains("crystal_"))
            {
                price = GetCrystalPrice(thing);
            }
            else if (thing.trait != null && PriceFuncByTraitDict.ContainsKey(thing.trait.GetType()))
            {
                price = PriceFuncByTraitDict[thing.trait.GetType()](thing);
            }

            return price * num;
        }

        public static Dictionary<Type, Func<Thing, int>> PriceFuncByTraitDict = new Dictionary<Type, Func<Thing, int>>()
        {
            { typeof(TraitRune), t => GetRunePrice(t) },
            { typeof(TraitMaterialHammer), t => GetHammerPrice(t) },
            { typeof(TraitBill), t => GetBillPrice(t) },
            { typeof(TraitRecipeCat), t => GetRecipePrice(t) },
            { typeof(TraitCurrencyMedal), t => GetMedalPrice(t) },
            { typeof(TraitCurrency), t => GetCurrencyPrice(t) },
            { typeof(TraitDrink), t => GetPotionPrice(t) },
            { typeof(TraitPotion), t => GetPotionPrice(t) },
            { typeof(TraitPotionRandom), t => GetPotionPrice(t) },
            { typeof(TraitPotionLove), t => GetPotionPrice(t) },
            { typeof(TraitPotionAlchemy), t => GetPotionPrice(t) },
            { typeof(TraitSpellbook), t => GetSpellBookPrice(t) },
            { typeof(TraitSpellbookRandom), t => GetSpellBookPrice(t) },
            { typeof(TraitRod), t => GetRodPrice(t) },
            { typeof(TraitRodRandom), t => GetRodPrice(t) },
            { typeof(TraitScroll), t => GetScrollPrice(t) },
            { typeof(TraitScrollRandom), t => GetScrollPrice(t) }
        };

        internal static int GetGearPrice(in Thing weapon)
        {
            return (weapon.rarity == Rarity.Mythical) ? 60 : 50;
        }

        internal static int GetCrystalPrice(in Thing clystal)
        {
            if (clystal.id == "crystal_earth")
            {
                return 3;
            }
            else if (clystal.id == "crystal_sun")
            {
                return 4;
            }

            return 5;
        }

        internal static int GetRunePrice(in Thing rune)
        {
            if (rune.refVal == SKILL.lockpicking || rune.refVal == SKILL.stealing)
            {
                return 10;
            }

            if (rune.encLV < 0)
            {
                return 8;
            }

            return 50;
        }

        internal static int GetHammerPrice(in Thing hammer)
        {
            int hammerTier = hammer.material.tier;
            int hammerHardness = hammer.material.hardness;
            int hammerWeightMtp = hammer.material.weight;
            int hammerPerformance = hammer.material.atk + hammer.material.dmg + hammer.material.dv + hammer.material.pv;

            int tierValue = hammerTier * 4;
            int hardnessValue = (hammerHardness / 20) * 2;
            int weightValue = (hammerWeightMtp < 50) ? (100 - hammerWeightMtp) / 5 :
                              (hammerWeightMtp >= 150 && hammerWeightMtp < 300) ? (300 - hammerWeightMtp) / 30 :
                              (hammerWeightMtp >= 300) ? (hammerWeightMtp - 300) / 20 : 0;
            int performanceValue = (hammerPerformance) / 6;

            int value = (tierValue + hardnessValue + weightValue + performanceValue + 5) / 2 * 2;

            // 金は該当するオパ像があればなんでも交換可能なので45固定
            if (hammer.material.alias == "gold")
            {
                return 45;
            }
            // 生ものは腐らせる用途があって特殊なので20固定
            if (hammer.material.alias == "meat")
            {
                return 20;
            }
            // 麻はクラフト用のありふれ付与素材として特殊なので20固定
            if (hammer.material.alias == "hemp")
            {
                return 20;
            }
            // シルクはクラフト用の回復力付与素材として特殊なので20固定
            if (hammer.material.alias == "silk")
            {
                return 20;
            }
            // 藁はクラフト用の素材として特殊なので16固定
            if (hammer.material.alias == "straw")
            {
                return 16;
            }
            // 綿はクラフト用の回復力付与素材として特殊なので16固定
            if (hammer.material.alias == "cotton")
            {
                return 16;
            }

            return Math.Max(value, 10);
        }

        internal static int GetBillPrice(in Thing bill)
        {
            return 7;
        }

        internal static int GetRecipePrice(in Thing recipe)
        {
            return 4;
        }

        internal static int GetMedalPrice(in Thing currency)
        {
            return 3;
        }

        internal static int GetCurrencyPrice(in Thing currency)
        {
            return 1;
        }

        internal static int GetPotionPrice(in Thing potion)
        {
            return (potion.id == "1165") ? 7 : 2;
        }

        internal static int GetSpellBookPrice(in Thing spellBook)
        {
            int refSpell = spellBook.refVal;

            if (refSpell == SPELL.SpHealJure || refSpell == SPELL.SpRebirth || refSpell == SPELL.SpWish)
            {
                return 10;
            }

            if (EClass.sources.elements.rows.Where(r => r.id == refSpell).First().tag.Contains("noShop"))
            {
                return 6;
            }

            return 4;
        }

        internal static int GetRodPrice(in Thing rod)
        {
            int refSpell = rod.refVal;

            if (rod.id == "rod_wish")
            {
                return (rod.c_charges == 0) ? 7 : 77;
            }

            if (EClass.sources.elements.rows.Where(r => r.id == refSpell).First().tag.Contains("noShop"))
            {
                return 6;
            }

            return 4;
        }

        internal static int GetScrollPrice(in Thing scroll)
        {
            if (scroll.refVal == SPELL.SpReconstruction)
            {
                return 7;
            }
            else if (scroll.refVal == SPELL.SpLighten)
            {
                return 3;
            }
            return 2;
        }
    }
}
