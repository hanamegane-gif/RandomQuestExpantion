using System.Linq;

namespace RandomQuestExpantion.Guilpo
{
    class PriceSetting
    {
        internal static int GetPrice(in Thing thing, int num)
        {
            int price = 1;
            if (thing.IsWeapon)
            {
                price = GetWeaponPrice(thing);
            }
            else if (thing.trait is TraitRune)
            {
                price = GetRunePrice(thing);
            }
            else if (thing.trait is TraitMaterialHammer)
            {
                price = GetHammerPrice(thing);
            }
            else if (thing.trait is TraitBill)
            {
                price = GetBillPrice(thing);
            }
            else if (thing.trait is TraitRecipeCat)
            {
                price = GetRecipePrice(thing);
            }
            else if (thing.trait is TraitCurrency)
            {
                price = GetCurrencyPrice(thing);
            }
            else if (thing.trait is TraitDrink)
            {
                price = GetPotionPrice(thing);
            }
            else if (thing.trait is TraitSpellbook)
            {
                price = GetSpellBookPrice(thing);
            }
            else if (thing.trait is TraitRod)
            {
                price = GetRodPrice(thing);
            }
            else if (thing.trait is TraitScrollRandom)
            {
                price = GetScrollPrice(thing);
            }
            else if (thing.id.Contains("crystal_"))
            {
                price = GetCrystalPrice(thing);
            }

            return price * num;
        }

        internal static int GetWeaponPrice(in Thing weapon)
        {
            return (weapon.rarity == Rarity.Mythical) ? 60 : 50;
        }

        internal static int GetRunePrice(in Thing rune)
        {
            if (rune.refVal == SKILL.lockpicking || rune.refVal == SKILL.stealing)
            {
                return 10;
            }
            return 50;
        }

        internal static int GetHammerPrice(in Thing hammer)
        {
            return 10;
        }

        internal static int GetBillPrice(in Thing bill)
        {
            return 7;
        }

        internal static int GetRecipePrice(in Thing recipe)
        {
            return 4;
        }

        internal static int GetCurrencyPrice(in Thing currency)
        {
            return (currency.trait is TraitCurrencyMedal) ? 3 : 1;
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

            return 3;
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

            return 3;
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
    }
}
