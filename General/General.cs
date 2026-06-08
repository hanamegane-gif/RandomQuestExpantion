using System.Collections.Generic;
using System.Linq;

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
        internal static void RemoveEnchantRandomOne(Thing gear)
        {
            var baseEnchants = gear.source.elementMap;
            var relpaceTargetEnchant = gear.elements.dict.Values.Where(e => (e._source.category == "attribute" || e._source.category == "skill" || e._source.category == "enchant") && !baseEnchants.ContainsKey(e.id)).RandomItem();

            if (relpaceTargetEnchant != null)
            {
                gear.elements.SetBase(relpaceTargetEnchant.id, 0);
            }
        }
    }
}
