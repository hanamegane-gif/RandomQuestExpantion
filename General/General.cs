using RandomQuestExpantion.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RandomQuestExpantion.General
{
    class General
    {
        internal static Zone FighterGuildZone => EClass.game.spatials.map.Values.Where(s => s.id == "kapul" && s.lv == -1).Cast<Zone>().First();
        internal static Zone MerchantGuildZone => EClass.game.spatials.map.Values.Where(s => s.id == "guild_merchant").Cast<Zone>().First();
        internal static Zone ThiefGuildZone => EClass.game.spatials.map.Values.Where(s => s.id == "derphy" && s.lv == -1).Cast<Zone>().First();
        internal static Zone MageGuildZone => EClass.game.spatials.map.Values.Where(s => s.id == "lumiest" && s.lv == -1).Cast<Zone>().First();

        internal static bool IsInGuild()
        {
            if (Guild.Fighter.IsCurrentZone || Guild.Mage.IsCurrentZone || Guild.Thief.IsCurrentZone || Guild.Merchant.IsCurrentZone)
            {
                return true;
            }

            return false;
        }

        internal static bool IsGuild(in Zone zone)
        {
            return IsFighterGuild(zone) || IsThiefGuild(zone) || IsMageGuild(zone) || IsMerchantGuild(zone);
        }

        internal static bool IsFighterGuild(in Zone zone)
        {
            return zone.id == "kapul" && zone.lv == -1;
        }

        internal static bool IsThiefGuild(in Zone zone)
        {
            return zone.id == "derphy" && zone.lv == -1;
        }

        internal static bool IsMageGuild(in Zone zone)
        {
            return zone.id == "lumiest" && zone.lv == -1;
        }

        internal static bool IsMerchantGuild(in Zone zone)
        {
            return zone.id == "guild_merchant";
        }

        internal static bool IsFighterGuildMember()
        {
            return Guild.Fighter.relation.type == FactionRelation.RelationType.Member;
        }

        internal static bool IsThiefGuildMember()
        {
            return Guild.Thief.relation.type == FactionRelation.RelationType.Member;
        }

        internal static bool IsMageGuildMember()
        {
            return Guild.Mage.relation.type == FactionRelation.RelationType.Member;
        }

        internal static bool IsMerchantGuildMember()
        {
            return Guild.Merchant.relation.type == FactionRelation.RelationType.Member;
        }

        internal static void RemoveAllInhabitants(Map map)
        {
            var shouldDieUID = new List<int>();
            foreach (var chara in map.charas)
            {
                if (chara != null && !chara.IsPCFactionOrMinion)
                {
                    shouldDieUID.Add(chara.uid);
                }
            }

            foreach (int UID in shouldDieUID)
            {
                map.charas.Where(c => c.uid == UID).First().Destroy();
            }
        }

        internal static void RemoveAllMedals(Map map)
        {
            var shouldDieUID = new List<int>();
            foreach (var thing in map.things)
            {
                if (thing != null && thing.id == "medal")
                {
                    shouldDieUID.Add(thing.uid);
                }
            }

            foreach (int UID in shouldDieUID)
            {
                map.things.Where(c => c.uid == UID).First().Destroy();
            }
        }

        internal static void RemoveAllStairs(Map map)
        {
            var shouldDieUID = new List<int>();
            foreach (var thing in map.things)
            {
                if (thing != null && (thing.source.trait.Contains("StairsDown") || thing.source.trait.Contains("StairsUp")))
                {
                    shouldDieUID.Add(thing.uid);
                }
            }

            foreach (int UID in shouldDieUID)
            {
                map.things.Where(c => c.uid == UID).First().Destroy();
            }
        }


        internal static Thing AddBonusRareEnchants(Thing gear, int enchantNum, int generateLv, Func<Func<SourceElement.Row, bool>> rareEnchantFilter)
        {
            // 素材エンチャが破壊に巻き込まれないようにするため一旦ダークマターにする
            var originalMaterial = gear.material;
            gear.ChangeMaterial("void");

            for (int i = 0; i < enchantNum; i++)
            {
                RemoveEnchantRandomOne(gear);
            }

            for (int i = 0; i < enchantNum; i++)
            {
                var bonusEnchant = PickBonusEnchant(gear, generateLv, rareEnchantFilter());
                if (bonusEnchant == null)
                {
                    continue;
                }

                int bonusEnchantStrength = CalcEnchantMagnitude(bonusEnchant, generateLv);
                gear.elements.ModBase(bonusEnchant.id, bonusEnchantStrength);
            }

            gear.ChangeMaterial(originalMaterial);

            return gear;
        }


        internal static SourceElement.Row PickBonusEnchant(in Thing thing, int generateLv, Func<SourceElement.Row, bool> rareEnchantFilter)
        {
            bool isRuneVessel = (thing.trait is TraitRune);

            if (!isRuneVessel && EClass.rnd(100) == 0 && ModConfig.EnableRuneVesselFeature)
            {
                return EClass.sources.elements.rows.Where(r => r.alias == "slot_rune").FirstOrDefault();
            }

            var gearCategory = thing.category;
            // chanceによる抽選は残しつつレアエンチャは出やすくする
            // フラグ系エンチャがボーナスで付くのはかわいそうなので弾いておく
            int sumChance = 0;
            var candidateList = new List<SourceElement.Row>();
            foreach (var enchant in EClass.sources.elements.rows.Where(r =>
                                                                            (isRuneVessel || r.IsEncAppliable(gearCategory)) &&
                                                                            !r.tag.Contains("flag") &&
                                                                            rareEnchantFilter(r) &&
                                                                            !r.tag.Contains("unused"))
            )
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

        internal static void RemoveEnchantRandomOne(Thing gear)
        {
            var baseEnchants = gear.source.elementMap;
            var relpaceTargetEnchant = gear.elements.dict.Values.Where(e => (e._source.category == "attribute" || e._source.category == "skill" || e._source.category == "enchant") && !baseEnchants.ContainsKey(e.id)).RandomItem();

            if (relpaceTargetEnchant != null)
            {
                gear.elements.SetBase(relpaceTargetEnchant.id, 0);
            }
        }

        internal static int CalcEnchantMagnitude(in SourceElement.Row enchant, int generateLv, int minPercent = 70)
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

            int maxMagnitude = linear + (int)Mathf.Sqrt(curvy);
            int maxPercent = Mathf.Clamp(100 - minPercent, 0, 100);


            int strength = (maxMagnitude * minPercent / 100) + EClass.rnd(1 + maxMagnitude * maxPercent / 100);
            strength = (enchant.mtp + strength) / enchant.mtp;

            if (enchant.encFactor == 0 && strength > 25)
            {
                strength = 25;
            }

            return strength;
        }

        internal static string GetElementText(in SourceElement.Row row, int Lv)
        {
            string[] textArray = row.GetTextArray("textAlt");
            int textIndex = Mathf.Clamp(Lv / 10 + 1, (Lv < 0 || textArray.Length <= 2) ? 1 : 2, textArray.Length - 1);
            string text = "altEnc".lang("", textArray[textIndex], "");

            return text;
        }

        internal static string AltTextDeadline(int publicationHours, int extraDays)
        {
            string publication = (publicationHours < 24) ? "byakko_mod_hour_abbr".lang(publicationHours.ToString()) : (publicationHours / 24).ToString();
            return publication + "+" + Date.GetText(extraDays * 24);
        }
    }
}
