using HarmonyLib;
using RandomQuestExpantion.ModQuests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.Patch
{
    // ギルド員はほぼ中立なので話しかけやすいように友好にしておく
    // 交換所と掲示板をギルドに設置する
    // ギルド連絡員を街に配置する
    // 高リスク護衛・配達クエスト中のエンカウント処理オーバーライドを実装する
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

            if (IsGuild(EClass._zone))
            {
                TryMakeFriendlyGuild(EClass._zone);
                TrySweepDebris(EClass._zone);
                TryDestroyFlyingQuestEquipment(EClass._zone);
                TryInstallQuestEquipment(EClass._zone);
            }

            /*
            if (EClass._zone.IsTown && EClass._zone.lv == 0)
            {
                TrySpawnGuildLiaison(EClass._zone);
            }
            */
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Player), nameof(Player.EnterLocalZone), new Type[] { typeof(Point), typeof(ZoneTransition), typeof(bool), typeof(Chara) })]
        internal static IEnumerable<CodeInstruction> QuestRelatedEncounterPatch(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var cm = new CodeMatcher(instructions, il)
            .MatchStartForward
            (
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Chara), nameof(Chara.MoveZone), new Type[] { typeof(Zone), typeof(ZoneTransition) }))
            );

            var continueLabel = il.DefineLabel();
            cm.Advance(1).Instruction.labels.Add(continueLabel);

            var ci =
            cm.InsertAndAdvance
            (
                new CodeInstruction(OpCodes.Ldarg_3),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(HasEncounterOverridesQuests))),
                new CodeInstruction(OpCodes.Brfalse_S, continueLabel),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(HandleOnWildernessEncounteredEvent))),
                new CodeInstruction(OpCodes.Ret)
            )
            .InstructionEnumeration();
            return ci;
        }

        internal static void FlyerBegone(Zone zone)
        {
            // 一度受け取ったチラシはずっとそのままになるので全て壊すんだ
            var civili = zone.map.charas.Where(c => !c.IsPCFactionOrMinion);
            foreach (var chara in civili)
            {
                chara.things.Find(t => t.id == "flyer")?.Destroy();
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

            string guildName = "???";
            foreach (var item in GuildZoneDict)
            {
                if (item.Value(guildZone))
                {
                    guildName = item.Key;
                }
                    
            }
            bool hasBoard = guildMap.things.Where(t => t.trait is TraitQuestBoard).Any();
            bool hasVender = guildMap.things.Where(t => t.trait is TraitGuilpoVender).Any();

            if (!hasBoard)
            {
                var coordinate = BoardCoordinateDict[guildName];
                int direction = BoardDirectionDict[guildName];

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
                var coordinate = VenderCoordinateDict[guildName];

                int direction = VenderDirectionDict[guildName];

                string thingId = VenderIdDict[guildName];

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

        internal static void TrySpawnGuildLiaison(Zone townZone)
        {
            foreach (var guild in GuildList)
            {
                SpawnLiaison(LiaisonIdDict[guild], LiaisonPosDict[guild]);
            }


            void SpawnLiaison(string liaisonID, Dictionary<string, Tuple<int, int, int>>  posDict)
            {
                if (!posDict.ContainsKey(townZone.id))
                {
                    return;
                }
                int count = townZone.map.charas.Concat(townZone.map.deadCharas).Where(c => c.id == liaisonID).Count();

                int xPos = posDict[townZone.id].Item1;
                int zPos = posDict[townZone.id].Item2;
                int numLimit = posDict[townZone.id].Item3;

                for (int i = 0; i < numLimit - count; i++)
                {
                    var spawnPos = new Point(xPos, zPos).GetRandomPointInRadius(2, 4);
                    var chara = CharaGen.Create(liaisonID);
                    EClass._zone.AddCard(chara, new Point(xPos, zPos));

                    // homeを設定してGlobalから取り除くことで街の住民のように自動蘇生できる
                    chara.SetHomeZone(townZone);
                    chara.RemoveGlobal();
                }
            }
        }

        internal static bool HasEncounterOverridesQuests(bool encounter)
        {
            return encounter && EClass.game.quests.list.Where(q => q is IOverrideWildernessEncounter && (q as IOverrideWildernessEncounter).ShouldOverrideEncounter()).Any();
        }

        internal static void HandleOnWildernessEncounteredEvent(Zone newZone)
        {
            // 難易度がおかしなことになるので複数クエストある場合はイベントを同時に発生させない
            var quest = EClass.game.quests.list.Where(q => q is IOverrideWildernessEncounter)
                                               .OrderBy(q => (q as IOverrideWildernessEncounter).GetOverlappingPriority())
                                               .FirstOrDefault();

            quest?.OnWildernessEncountedMod(newZone);
        }

        #region きたない
        class Direction
        {
            internal static int Vertical => 0;
            internal static int Horizontal => 1;
        }

        private static HashSet<string> GuildList { get; } = new HashSet<string>
        {
            "fighter","merchant","thief","mage",
        };

        private static Dictionary<string, Func<Zone, bool>> GuildZoneDict { get; } = new Dictionary<string, Func<Zone, bool>>
        {
            { "fighter", z => IsFighterGuild(z) },
            { "merchant", z => IsMerchantGuild(z) },
            { "thief", z => IsThiefGuild(z) },
            { "mage", z => IsMageGuild(z) },
        };

        private static Dictionary<string, Point> BoardCoordinateDict { get; } = new Dictionary<string, Point>
        {
            { "fighter", new Point(53, 91) },
            { "merchant", new Point(42, 51) },
            { "thief", new Point(65, 80) },
            { "mage", new Point(36, 91) },
        };

        private static Dictionary<string, int> BoardDirectionDict { get; } = new Dictionary<string, int>
        {
            { "fighter", Direction.Vertical },
            { "merchant", Direction.Vertical },
            { "thief", Direction.Vertical },
            { "mage", Direction.Vertical },
        };

        private static Dictionary<string, string> VenderIdDict { get; } = new Dictionary<string, string>
        {
            { "fighter", "MOD_byakko_RQX_guilpo_exc_fighter" },
            { "merchant", "MOD_byakko_RQX_guilpo_exc_merchant" },
            { "thief", "MOD_byakko_RQX_guilpo_exc_thief" },
            { "mage", "MOD_byakko_RQX_guilpo_exc_mage" },
        };

        private static Dictionary<string, Point> VenderCoordinateDict { get; } = new Dictionary<string, Point>
        {
            { "fighter", new Point(67, 91) },
            { "merchant", new Point(47, 50) },
            { "thief", new Point(61, 78) },
            { "mage", new Point(45, 91) },
        };

        private static Dictionary<string, int> VenderDirectionDict { get; } = new Dictionary<string, int>
        {
            { "fighter", Direction.Horizontal },
            { "merchant", Direction.Vertical },
            { "thief", Direction.Vertical },
            { "mage", Direction.Horizontal },
        };

        private static Dictionary<string, string> LiaisonIdDict { get; } = new Dictionary<string, string>
        {
            { "fighter", "liaisonfg" },
            { "merchant", "liaisonmg" },
            { "thief", "liaisontg" },
            { "mage", "liaisonwg" },
        };

        private static Dictionary<string, Dictionary<string, Tuple<int, int, int>>> LiaisonPosDict { get; } = new Dictionary<string, Dictionary<string, Tuple<int, int, int>>>
        {
            { "fighter", FGLiaisonPosDict },
            { "merchant", MGLiaisonPosDict },
            { "thief", TGLiaisonPosDict },
            { "mage", MGLiaisonPosDict },
        };

        private static Dictionary<string, Tuple<int, int, int>> FGLiaisonPosDict { get; } = new Dictionary<string, Tuple<int, int, int>>
        {
            { "kapul", new Tuple<int, int, int>(61, 96, 0) },
            { "tinkerCamp", new Tuple<int, int, int>(56, 43, 1) },
            { "olvina", new Tuple<int, int, int>(34, 59, 1) },
            { "aquli", new Tuple<int, int, int>(58, 64, 2) },
            { "yowyn", new Tuple<int, int, int>(46, 49, 1) },
            { "specwing", new Tuple<int, int, int>(52, 42, 1) },
            { "village_exile", new Tuple<int, int, int>(35, 52, 0) },
            { "foxtown", new Tuple<int, int, int>(39, 40, 1) },
            { "foxtown_nefu", new Tuple<int, int, int>(33, 39, 1) },
            { "palmia", new Tuple<int, int, int>(70, 69, 2) },
            { "lothria", new Tuple<int, int, int>(60, 57, 2) },
            { "mysilia", new Tuple<int, int, int>(46, 100, 2) },
            { "derphy", new Tuple<int, int, int>(58, 71, 2) },
            { "lumiest", new Tuple<int, int, int>(78, 62, 1) },
            { "noyel", new Tuple<int, int, int>(37, 58, 2) },
        };

        private static Dictionary<string, Tuple<int, int, int>> MGLiaisonPosDict { get; } = new Dictionary<string, Tuple<int,int,int>>
        {
            { "kapul", new Tuple<int, int, int>(54, 75, 2) },
            { "tinkerCamp", new Tuple<int, int, int>(50, 53, 1) },
            { "olvina", new Tuple<int, int, int>(50, 42, 1) },
            { "aquli", new Tuple<int, int, int>(54, 55, 1) },
            { "yowyn", new Tuple <int, int, int>(40, 57, 1) },
            { "specwing", new Tuple <int, int, int>(50, 51, 1)  },
            { "village_exile", new Tuple <int, int, int>(55, 52, 0) },
            { "foxtown", new Tuple <int, int, int>(55, 46, 2) },
            { "foxtown_nefu", new Tuple <int, int, int>(56, 63, 2) },
            { "palmia", new Tuple <int, int, int>(73, 55, 2) },
            { "lothria", new Tuple <int, int, int>(47, 49, 1) },
            { "mysilia", new Tuple <int, int, int>(78, 78, 2) },
            { "derphy", new Tuple <int, int, int>(93, 70, 1) },
            { "lumiest", new Tuple <int, int, int>(61, 57, 1) },
            { "noyel", new Tuple <int, int, int>(43, 73, 1) },
        };

        private static Dictionary<string, Tuple<int, int, int>> TGLiaisonPosDict { get; } = new Dictionary<string, Tuple<int, int, int>>
        {
            { "kapul", new Tuple<int, int, int>(76, 68, 2) },
            { "tinkerCamp", new Tuple<int, int, int>(57, 64, 1) },
            { "olvina", new Tuple<int, int, int>(64, 45, 1) },
            { "aquli", new Tuple <int, int, int>(36, 36, 1) },
            { "yowyn", new Tuple <int, int, int>(50, 39, 1) },
            { "specwing", new Tuple <int, int, int>(61, 46, 1) },
            { "village_exile", new Tuple <int, int, int>(51, 33, 0) },
            { "foxtown", new Tuple <int, int, int>(61, 59, 1) },
            { "foxtown_nefu", new Tuple <int, int, int>(40, 59, 1) },
            { "palmia", new Tuple <int, int, int>(28, 55, 2) },
            { "lothria", new Tuple <int, int, int>(59, 40, 1) },
            { "mysilia", new Tuple <int, int, int>(75, 46, 2) },
            { "derphy", new Tuple <int, int, int>(68, 70, 0) },
            { "lumiest", new Tuple <int, int, int>(58, 105, 1) },
            { "noyel", new Tuple <int, int, int>(90, 52, 1) },
        };

        private static Dictionary<string, Tuple<int, int, int>> WGLiaisonPosDict { get; } = new Dictionary<string, Tuple<int, int, int>>
        {
            { "kapul", new Tuple<int, int, int>(50, 95, 2) },
            { "tinkerCamp", new Tuple<int, int, int>(64, 51, 1) },
            { "olvina", new Tuple<int, int, int>(53, 53, 1) },
            { "aquli", new Tuple<int, int, int>(48, 53, 1) },
            { "yowyn", new Tuple<int, int, int>(33, 41, 1) },
            { "specwing", new Tuple<int, int, int>(58, 59, 1) },
            { "village_exile", new Tuple<int, int, int>(39, 63, 0) },
            { "foxtown", new Tuple<int, int, int>(39, 58, 1) },
            { "foxtown_nefu", new Tuple<int, int, int>(58, 46, 2) },
            { "palmia", new Tuple<int, int, int>(54, 44, 2) },
            { "lothria", new Tuple<int, int, int>(39, 50, 1) },
            { "mysilia", new Tuple<int, int, int>(50, 70, 2) },
            { "derphy", new Tuple<int, int, int>(71, 79, 1) },
            { "lumiest", new Tuple<int, int, int>(41, 92, 0) },
            { "noyel", new Tuple<int, int, int>(75, 87, 2) },
        };

        #endregion
    }
}
