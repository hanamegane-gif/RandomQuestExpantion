using HarmonyLib;
using RandomQuestExpantion.ModQuests.Common;
using System.Linq;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.Patch
{
    // ギルド員はほぼ中立なので話しかけやすいように友好にしておく
    // 交換所と掲示板をギルドに設置する
    [HarmonyPatch]
    class PlayerPatch
    {
        [HarmonyPatch(typeof(Player.Flags), nameof(Player.Flags.OnEnterZone)), HarmonyPostfix]
        internal static void FriendlyGuildPatch()
        {
            if (EClass.game.quests.list.Any(q => q is QuestFlyer) && EClass._zone is Zone_Civilized)
            {
                FlyerBegone(EClass._zone);
            }
            if (!IsGuild(EClass._zone))
            {
                return;
            }

            TryMakeFriendlyGuild(EClass._zone);
            TrySweepDebris(EClass._zone);
            TryDestroyFlyingQuestEquipment(EClass._zone);
            TryInstallQuestEquipment(EClass._zone);
        }

        internal static void FlyerBegone(Zone zone)
        {
            // 一度受け取ったチラシはずっとそのままになるので全て壊すんだ
            var civili = zone.map.charas.Where(c => !c.IsPCFactionOrMinion);
            foreach (var chara in civili)
            {
                var flyer = chara.things.Find(t => t.id == "flyer");
                if (flyer != null)
                {
                    flyer.Destroy();
                }
            }
        }

        internal static void TryMakeFriendlyGuild(Zone guildZone)
        {
            var guildMap = guildZone.map;
            foreach (var chara in guildMap.charas)
            {
                if (chara.OriginalHostility == Hostility.Neutral && !chara.IsPCFactionOrMinion && !chara.IsMinion && chara.IsHuman)
                {
                    chara.c_originalHostility = Hostility.Friend;
                    chara.hostility = Hostility.Friend;
                }
            }
        }

        internal static void TryInstallQuestEquipment(Zone guildZone)
        {
            var guildMap = guildZone.map;

            var hasBoard = guildMap.things.Where(t => t.trait is TraitQuestBoard).Any();
            var hasVender = guildMap.things.Where(t => t.trait is TraitGuilpoVender).Any();

            if (!hasBoard)
            {
                Point coordinate = (IsFighterGuild(guildZone)) ? FGBoardCoordinate :
                                   (IsMerchantGuild(guildZone)) ? MGBoardCoordinate :
                                   (IsThiefGuild(guildZone)) ? TGBoardCoordinate :
                                   (IsMageGuild(guildZone)) ? WGBoardCoordinate : null;
                int direction = (IsFighterGuild(guildZone)) ? FGBoardDirection :
                                (IsMerchantGuild(guildZone)) ? MGBoardDirection :
                                (IsThiefGuild(guildZone)) ? TGBoardDirection :
                                (IsMageGuild(guildZone)) ? WGBoardDirection : 0;

                if (coordinate != null)
                {
                    var board = ThingGen.Create("4");
                    board.dir = direction;
                    board.isNPCProperty = true;
                    guildZone.AddCard(board, coordinate).Install().ignoreStackHeight = true;
                }
            }

            if (!hasVender)
            {
                var coordinate = (IsFighterGuild(guildZone)) ? FGVenderCoordinate :
                                   (IsMerchantGuild(guildZone)) ? MGVenderCoordinate :
                                   (IsThiefGuild(guildZone)) ? TGVenderCoordinate :
                                   (IsMageGuild(guildZone)) ? WGVenderCoordinate : null;

                int direction = (IsFighterGuild(guildZone)) ? FGVenderDirection :
                                (IsMerchantGuild(guildZone)) ? MGVenderDirection :
                                (IsThiefGuild(guildZone)) ? TGVenderDirection :
                                (IsMageGuild(guildZone)) ? WGVenderDirection : 0;

                var thingId = (IsFighterGuild(guildZone)) ? FGVenderID :
                              (IsMerchantGuild(guildZone)) ? MGVenderID :
                              (IsThiefGuild(guildZone)) ? TGVenderID :
                              (IsMageGuild(guildZone)) ? WGVenderID : "";

                if (coordinate != null)
                {
                    var vender = ThingGen.Create(thingId);
                    vender.dir = direction;
                    vender.isNPCProperty = true;
                    guildZone.AddCard(vender, coordinate).Install().ignoreStackHeight = true;
                }
            }
        }

        // 家具設置挙動の変更で空を飛んだギルポ交換所を破壊し、地面に再設置
        internal static void TryDestroyFlyingQuestEquipment(Zone guildZone)
        {
            var venders = guildZone.map.things.Where(t => t.trait is TraitGuilpoVender && !t.ignoreStackHeight).ToList();
            for (int i = venders.Count() - 1; i >= 0; i--)
            {
                venders[i].Destroy();
            }
        }

        // Modを抜いた際に発生するギルポ交換所の錬金灰を除去する
        internal static void TrySweepDebris(Zone guildZone)
        {
            var deblis = guildZone.map.things.Where(t => t.id == "ash3" && t.Quality == 4).ToList();
            for (int i = deblis.Count() - 1; i >= 0; i--)
            {
                deblis[i].Destroy();
            }
        }

        class Direction
        {
            internal static int Vertical => 0;
            internal static int Horizontal => 1;
        }

        private static Point FGBoardCoordinate { get; } = new Point(53, 91);
        private static Point FGVenderCoordinate { get; } = new Point(67, 91);
        private static int FGBoardDirection { get; } = Direction.Vertical;
        private static int FGVenderDirection { get; } = Direction.Horizontal;
        private static string FGVenderID { get; } = "MOD_byakko_RQX_guilpo_exc_fighter";
        private static Point MGBoardCoordinate { get; } = new Point(42, 51);
        private static Point MGVenderCoordinate { get; } = new Point(47, 50);
        private static int MGBoardDirection { get; } = Direction.Vertical;
        private static int MGVenderDirection { get; } = Direction.Vertical;
        private static string MGVenderID { get; } = "MOD_byakko_RQX_guilpo_exc_merchant";
        private static Point TGBoardCoordinate { get; } = new Point(65, 80);
        private static Point TGVenderCoordinate { get; } = new Point(61, 78);
        private static int TGBoardDirection { get; } = Direction.Vertical;
        private static int TGVenderDirection { get; } = Direction.Vertical;
        private static string TGVenderID { get; } = "MOD_byakko_RQX_guilpo_exc_thief";
        private static Point WGBoardCoordinate { get; } = new Point(36, 91);
        private static Point WGVenderCoordinate { get; } = new Point(45, 91);
        private static int WGBoardDirection { get; } = Direction.Vertical;
        private static int WGVenderDirection { get; } = Direction.Horizontal;
        private static string WGVenderID { get; } = "MOD_byakko_RQX_guilpo_exc_mage";
    }
}
