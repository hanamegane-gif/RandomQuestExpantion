using HarmonyLib;
using RandomQuestExpantion.General;
using RandomQuestExpantion.ModQuests.Common;
using RandomQuestExpantion.ModQuests.QuestAttribute;
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

            if (EClass._zone.IsTown && EClass._zone.lv == 0)
            {
                TrySpawnGuildLiaison(EClass._zone);
            }
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
                new CodeInstruction(OpCodes.Ldarg, 4),
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
                int direction = (int)BoardDirectionDict[guildName];

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

                int direction = (int)VenderDirectionDict[guildName];

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
            SpawnLiaison(TraitFGLiaison.LiaisonId, TraitFGLiaison.LiaisonPosDict);
            SpawnLiaison(TraitMGLiaison.LiaisonId, TraitMGLiaison.LiaisonPosDict);
            SpawnLiaison(TraitTGLiaison.LiaisonId, TraitTGLiaison.LiaisonPosDict);
            SpawnLiaison(TraitWGLiaison.LiaisonId, TraitWGLiaison.LiaisonPosDict);

            void SpawnLiaison(string liaisonID, Dictionary<string, Tuple<int, int, int>> posDict)
            {
                if (!posDict.ContainsKey(townZone.id))
                {
                    return;
                }
                var deads = townZone.map?.charas;
                if (deads == null)
                {
                    deads = new List<Chara>();
                }
                int count = townZone.map.charas.Concat(deads).Where(c => c.id == liaisonID).Count();

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

        internal static bool HasEncounterOverridesQuests(bool encounter, Chara mob)
        {
            return encounter && mob == null && EClass.game.quests.list.Where(q => q is IOverrideWildernessEncounter && (q as IOverrideWildernessEncounter).ShouldOverrideEncounter()).Any();
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

        private static Dictionary<string, Func<Zone, bool>> GuildZoneDict => new Dictionary<string, Func<Zone, bool>>
        {
            { "fighter", z => IsFighterGuild(z) },
            { "merchant", z => IsMerchantGuild(z) },
            { "thief", z => IsThiefGuild(z) },
            { "mage", z => IsMageGuild(z) },
        };

        private static Dictionary<string, Point> BoardCoordinateDict => new Dictionary<string, Point>
        {
            { "fighter", new Point(53, 91) },
            { "merchant", new Point(42, 51) },
            { "thief", new Point(65, 80) },
            { "mage", new Point(36, 91) },
        };

        private static Dictionary<string, Direction> BoardDirectionDict => new Dictionary<string, Direction>
        {
            { "fighter", Direction.Vertical },
            { "merchant", Direction.Vertical },
            { "thief", Direction.Vertical },
            { "mage", Direction.Vertical },
        };

        private static Dictionary<string, string> VenderIdDict => new Dictionary<string, string>
        {
            { "fighter", "MOD_byakko_RQX_guilpo_exc_fighter" },
            { "merchant", "MOD_byakko_RQX_guilpo_exc_merchant" },
            { "thief", "MOD_byakko_RQX_guilpo_exc_thief" },
            { "mage", "MOD_byakko_RQX_guilpo_exc_mage" },
        };

        private static Dictionary<string, Point> VenderCoordinateDict => new Dictionary<string, Point>
        {
            { "fighter", new Point(67, 91) },
            { "merchant", new Point(47, 50) },
            { "thief", new Point(61, 78) },
            { "mage", new Point(45, 91) },
        };

        private static Dictionary<string, Direction> VenderDirectionDict => new Dictionary<string, Direction>
        {
            { "fighter", Direction.Horizontal },
            { "merchant", Direction.Vertical },
            { "thief", Direction.Vertical },
            { "mage", Direction.Horizontal },
        };
        #endregion
    }
}
