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

        internal static bool IsNefiaBoss(in Chara killedChara)
        {
            // ネフィアボス討伐時はzone.Bossをnullにした後にOnKillCharaが呼ばれるため、EClass._zone.Boss == killedCharaで楽に判定できない
            // まともにネフィアの主を判定する方法がないので力業

            // 「ネフィアの主＝最下層にいるボス」として、最下層かどうかはShouldMakeExitで判定する
            if (!EClass._zone.IsNefia || EClass._zone.ShouldMakeExit)
            {
                return false;
            }

            // 争いの祠で出てくるのもボス扱いなので引っかからないようにする
            if (EClass._zone.Boss != null)
            {
                return false;
            }

            return killedChara.c_bossType == BossType.Boss;
        }

        internal static void RemoveAllInhabitants(Map map)
        {
            List<int> shouldDieUID = new List<int>();
            foreach (var chara in map.charas)
            {
                if (chara != null && !chara.IsPCFaction)
                {
                    shouldDieUID.Add(chara.uid);
                }
            }

            foreach (var UID in shouldDieUID)
            {
                map.charas.Where(c => c.uid == UID).First().Destroy();
            }
        }

        internal static void RemoveAllMedals(Map map)
        {
            List<int> shouldDieUID = new List<int>();
            foreach (var thing in map.things)
            {
                if (thing != null && thing.id == "medal")
                {
                    shouldDieUID.Add(thing.uid);
                }
            }

            foreach (var UID in shouldDieUID)
            {
                map.things.Where(c => c.uid == UID).First().Destroy();
            }
        }

        internal static void RemoveAllStairs(Map map)
        {
            List<int> shouldDieUID = new List<int>();
            foreach (var thing in map.things)
            {
                if (thing != null && (thing.source.trait.Contains("StairsDown") || thing.source.trait.Contains("StairsUp")))
                {
                    shouldDieUID.Add(thing.uid);
                }
            }

            foreach (var UID in shouldDieUID)
            {
                map.things.Where(c => c.uid == UID).First().Destroy();
            }
        }
    }
}
