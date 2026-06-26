using System;
using System.Collections.Generic;
using UnityEngine;

namespace RandomQuestExpantion.Guilpo
{
    internal class StackSetting
    {
        internal static int GetStackNum(in Thing thing)
        {
            var trait = thing.trait;

            if (trait == null || !trait.CanStack)
            {
                return 1;
            }

            if (thing.trait != null && StackNumFuncByTraitDict.ContainsKey(thing.trait.GetType()))
            {
                return StackNumFuncByTraitDict[thing.trait.GetType()](thing);
            }

            return 1;

        }
        internal static int GetChargeNum(in Thing thing)
        {
            var trait = thing.trait;

            if (trait == null || !trait.HasCharges)
            {
                return 0;
            }

            if (thing.trait != null && ChargeNumFuncByTraitDict.ContainsKey(thing.trait.GetType()))
            {
                return ChargeNumFuncByTraitDict[thing.trait.GetType()](thing);
            }

            return 1;
        }

        public static Dictionary<Type, Func<Thing, int>> StackNumFuncByTraitDict = new Dictionary<Type, Func<Thing, int>>()
        {
            { typeof(TraitCurrencyMedal), t => GetMedalStackNum(t) },
            { typeof(TraitCurrency), t => GetCurrencyStackNum(t) },
            { typeof(TraitPotion), t => GetPotionNum(t) },
            { typeof(TraitScrollRandom), t => GetScrollNum(t) },
            { typeof(TraitMaterialHammer), t => GetMaterialHammerNum(t) },
        };

        public static Dictionary<Type, Func<Thing, int>> ChargeNumFuncByTraitDict = new Dictionary<Type, Func<Thing, int>>()
        {
            { typeof(TraitRodRandom), t => GetRodChargeNum(t) },
            { typeof(TraitSpellbookRandom), t => GetSpellbookChargeNum(t) },
        };

        public static int GetMedalStackNum(in Thing t)
        {
            return 10;
        }

        public static int GetCurrencyStackNum(in Thing t)
        {
            return 500;
        }

        public static int GetPotionNum(in Thing t)
        {
            return 100;
        }

        public static int GetScrollNum(in Thing t)
        {
            if (t.refVal == SPELL.SpLighten)
            {
                int progress = Mathf.Max(EClass.pc.FameLv, EClass.player.stats.deepest, 15);
                return (progress <= 500) ? 2 : Mathf.Min(1 + progress / 100, 100);
            }

            return 100;
        }

        public static int GetMaterialHammerNum(in Thing t)
        {
            return 1;
        }

        public static int GetRodChargeNum(in Thing t)
        {
            if (YabaiList.Contains(t.refVal))
            {
                return 1;
            }

            return 30;
        }

        public static int GetSpellbookChargeNum(in Thing t)
        {
            if (YabaiList.Contains(t.refVal))
            {
                return 1;
            }

            return 7;
        }

        private static HashSet<int> YabaiList => new HashSet<int>()
        {
            SPELL.SpWish,
            SPELL.SpHealJure,
            SPELL.SpRebirth,
        };
    }
}
