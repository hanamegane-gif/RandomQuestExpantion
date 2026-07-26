using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RandomQuestExpantion.Config;
using RandomQuestExpantion.General;
using RandomQuestExpantion.ModQuests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RandomQuestExpantion.ModQuestEvent
{
    internal class ZoneEventRoadDefence : ZoneEventDefenseGame
    {
        public override string TextWidgetDate => "byakko_mod_status_road_defence".lang(wave.ToString() ?? "", EClass._zone.DangerLv.ToString() ?? "", kills.ToString() ?? "", (WaveKillRequirements * WaveRequirements).ToString()) + ((instance != null && retreated) ? "defenseRetreating".lang() : "");

        [JsonProperty]
        public int WaveEnemyCounts = 0;

        public virtual int WaveRequirements => quest.difficulty * 2 / 7 + 4; // 1～3: 4, 4～6:5, 7:6

        public virtual int WaveKillRequirements => 15 + quest.difficulty * 3 / 2;

        public virtual int InitialDangerLV => quest.dangerLv;

        public int RoundUntilNextWave => 30; // 次Waveまで30 * 3.5Tの間隔

        public bool IsCompletable => !IsRemainsNextWave && kills >= WaveKillRequirements * WaveRequirements;

        public bool IsEngaging => enemies.Any();

        public bool IsRemainsNextWave => wave < WaveRequirements;

        public bool ShouldTriggerNextWave => CondiTooLate || CondiNoEnemy;

        public bool CondiNoEnemy => !IsEngaging;

        public bool CondiTooLate => RoundUntilNextWave <= turns;

        private Dictionary<string, int> _CandEnemiesCache = new Dictionary<string, int>();

        private int _CandChanceSum = 0;


        public override void OnVisit()
        {
            if (EClass.game.isLoading)
            {
                QuestRoadDefence.lastWave = wave;
                QuestRoadDefence.bonus = bonus;
                return;
            }
            EClass._zone._dangerLv = InitialDangerLV;

            MoveToEnterPosition();
            SetFieldPiece();
            SpawnReinforcement();

            EClass._zone.SetBGM(107);
            EClass._map.RevealAll();

            Msg.Say("defense_start");
            NextWave();
        }

        public override void _OnTickRound()
        {
            QuestRoadDefence.lastWave = wave;
            turns++;

            if (ShouldTriggerNextWave)
            {
                NextWave();
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    if (WaveEnemyCounts < WaveKillRequirements)
                    {
                        SpawnEnemy(1);
                    }
                }
                AggroEnemy();
                AggroGuards();
            }
        }

        public override void OnCharaDie(Chara c)
        {
            if (c.IsPCParty || c.IsPCPartyMinion || !enemies.Contains(c.uid))
            {
                return;
            }

            kills++;
            enemies.Remove(c.uid);
            QuestRoadDefence.bonus += CalcBonusMoney(c);

            if (!retreated && instance.status != ZoneInstance.Status.Success && IsCompletable)
            {
                SE.Play("warhorn");
                Msg.Say("warhorn");
                Msg.Say("defense_retreat");
                retreated = true;
                instance.status = ZoneInstance.Status.Success;
                ActEffect.Proc(EffectId.Evac, EClass.pc);
            }
        }

        internal virtual int CalcBonusMoney(in Chara killedChara)
        {
            int baseMoney = EClass.curve(killedChara.LV + 10, 50, 10, 50) / 2;
            return EClass.rndHalf(baseMoney);
        }

        internal virtual void SetFieldPiece()
        {
            var genBounds = GenBounds.Create(EClass._zone);
            genBounds.FuncCheckEmpty = (Cell cell) => cell.sourceFloor.id == 40;

            genBounds.marginPartial = 2;
            for (int i = 0; i < 100; i++)
            {
                ModMapPiece.TryAddMapPiece(genBounds, "road_defence");
            }

            genBounds.marginPartial = 1;
            for (int i = 0; i < 40; i++)
            {
                ModMapPiece.TryAddMapPiece(genBounds, "road_defence");
            }

            foreach (var thing in EClass._map.things)
            {
                thing.isNPCProperty = true;
            }
        }

        internal virtual void SpawnReinforcement()
        {
            var turretPlaces = new List<Point>();
            for (int z = EClass._map.bounds.z; z < EClass._map.bounds.maxZ; z++)
            {
                for (int x = EClass._map.bounds.x; x < EClass._map.bounds.maxX; x++)
                {
                    if (EClass._map.cells[x, z].sourceFloor.id == 120)
                    {
                        turretPlaces.Add(new Point(x, z));
                    }
                }
            }

            foreach (var pos in turretPlaces)
            {
                EClass._zone.AddCard(CreateTurret(), pos);
            }

            for (int i = 0; i < 5; i++)
            {
                // PCについていかず、個別にターゲティングする
                EClass._zone.AddCard(CreateGuard(), EClass.pc.pos.GetRandomPointInRadius(2, 4));
            }
        }

        private void MoveToEnterPosition()
        {
            var signPos = EClass._map.things.Where(t => t.id == "1224").FirstOrDefault().pos;

            foreach (var chara in EClass._map.charas)
            {
                if (!chara.HasHost)
                {
                    chara._Move(signPos.GetNearestPoint(allowBlock: false, allowChara: false));
                    chara.renderer.SetFirst(first: true, chara.pos.PositionCenter());
                }
            }

            EClass.screen.FocusPC();
            EClass.screen.RefreshPosition();
        }

        internal virtual void NextWave()
        {
            wave++;
            turns = 0;
            WaveEnemyCounts = 0;

            // 延長戦中のみ危険度を増やして徐々に諦めの気持ちにする
            if (!IsRemainsNextWave)
            {
                long nextLevel = EClass._zone.DangerLv + ((EClass._zone.DangerLv >= 100) ? EClass._zone.DangerLv / 100 * 5 : 0) + 5;
                EClass._zone._dangerLv = (int)Math.Min(nextLevel, 2000000000);

                // 候補の再取得を行うためにキャッシュ削除
                if (nextLevel < 109)
                {
                    _CandEnemiesCache.Clear();
                    _CandChanceSum = 0;
                }
            }

            SE.Play("warhorn");
            Msg.Say("warhorn");
            Msg.Say("defense_wave", wave.ToString() ?? "", EClass._zone.DangerLv.ToString() ?? "");

            if (wave % 3 == 0)
            {
                SpawnBoss();
            }

            SpawnEnemy(5 + quest.difficulty * 3 / 2);
            AggroEnemy();
            AggroGuards();
        }

        internal virtual void SpawnEnemy(int num = 1)
        {
            if (!_CandEnemiesCache.Any())
            {
                CacheCandEnemies();
            }

            for (int i = 0; i < num; i++)
            {
                string spawnId = "jure";
                int roll = EClass.rnd(_CandChanceSum);
                int sum = 0;
                foreach (var kvp in _CandEnemiesCache)
                {
                    sum += kvp.Value;
                    if (roll < sum)
                    {
                        spawnId = kvp.Key;
                        break;
                    }
                }

                // 東端から出す
                var pos = new Point(EClass._map.bounds.x, EClass._map.bounds.z + EClass.rnd(EClass._map.bounds.maxZ - EClass._map.bounds.z));
                var boundaryEdgePoint = pos.GetNearestPoint(allowBlock: false, allowChara: false, minRadius: 3); 

                var charaSource = EClass.sources.charas.rows.FirstOrDefault(r => r.id == spawnId);
                int charaLv = ((EClass._zone.DangerLv > 50) ? (charaSource.LV + 50) * (EClass._zone.DangerLv / 50) * 2 / 3 : charaSource.LV);

                var spawnedChara = CharaGen.Create(charaSource.id, charaLv);
                spawnedChara.SetLv(charaLv);
                spawnedChara.c_originalHostility = Hostility.Enemy;
                spawnedChara.hostility = Hostility.Enemy;

                EClass._zone.AddCard(spawnedChara, boundaryEdgePoint);
                enemies.Add(spawnedChara.uid);
                WaveEnemyCounts++;
            }
        }

        internal virtual void SpawnBoss()
        {
            if (!_CandEnemiesCache.Any())
            {
                CacheCandEnemies();
            }

            string spawnId = "jure";
            int roll = EClass.rnd(_CandChanceSum);
            int sum = 0;
            foreach (var kvp in _CandEnemiesCache)
            {
                sum += kvp.Value;
                if (roll < sum)
                {
                    spawnId = kvp.Key;
                    break;
                }
            }

            // 東端から出す
            var pos = new Point(EClass._map.bounds.x, EClass._map.bounds.z + EClass.rnd(EClass._map.bounds.maxZ - EClass._map.bounds.z));
            var boundaryEdgePoint = pos.GetNearestPoint(allowBlock: false, allowChara: false, minRadius: 3);

            var charaSource = EClass.sources.charas.rows.FirstOrDefault(r => r.id == spawnId);
            int charaLv = ((EClass._zone.DangerLv > 50) ? (charaSource.LV + 50) * (EClass._zone.DangerLv / 50) * 2 / 3 : charaSource.LV);

            var cardBlueprint = new CardBlueprint
            {
                rarity = Rarity.Legendary,
                lv = charaLv,
            };
            CardBlueprint.Set(cardBlueprint);


            var spawnedChara = CharaGen.Create(charaSource.id, charaLv);
            spawnedChara.SetLv(charaLv);
            spawnedChara.c_originalHostility = Hostility.Enemy;
            spawnedChara.hostility = Hostility.Enemy;

            EClass._zone.AddCard(spawnedChara, boundaryEdgePoint);
            enemies.Add(spawnedChara.uid);
            WaveEnemyCounts++;

            Msg.Say("defense_boss", spawnedChara.Name);
            EClass.game.Pause();
        }

        private new void AggroEnemy(int chance = 100)
        {
            var PCParty = EClass._map.charas.Where(c => c.IsPCFactionOrMinion).ToList();

            // タレットをターゲットにすると回り込むように移動するためめんどくさくなる
            var guards = EClass._map.charas.Where(c => c.trait is TraitGuard && c.c_originalHostility == Hostility.Friend && !c.IsPCFactionOrMinion).ToList();

            foreach (var chara in EClass._map.charas.Where(c => !c.IsPCFactionOrMinion && !c.IsInCombat && c.c_originalHostility == Hostility.Enemy))
            {
                if(chance < 100 && EClass.rnd(100) >= chance)
                {
                    continue;
                }

                if (guards.Any() && EClass.rnd(3) == 0)
                {
                    chara.SetEnemy(guards.RandomItem());
                }
                else
                {
                    chara.SetEnemy(PCParty.RandomItem());
                }
                chara.SetAIAggro();
            }
        }

        private void AggroGuards(int chance = 100)
        {
            var allEnemies = EClass._map.charas.Where(c => !c.IsPCFactionOrMinion && c.c_originalHostility == Hostility.Enemy).ToList();

            if (!allEnemies.Any())
            {
                return;
            }

            foreach (var chara in EClass._map.charas.Where(c => !c.IsPCFactionOrMinion && !c.IsInCombat && c.c_originalHostility == Hostility.Friend))
            {
                if (chance >= 100 || EClass.rnd(100) <= chance)
                {
                    chara.SetEnemy(allEnemies.RandomItem());
                    chara.SetAIAggro();
                }
            }
        }

        private Chara CreateTurret()
        {
            int charaLv = quest.dangerLv;
            var chara = CharaGen.Create("turret", charaLv);
            chara.SetLv(charaLv);
            chara.hostility = Hostility.Friend;
            chara.c_originalHostility = Hostility.Friend;

            // 当たらないわ火力低いわ魔法に脆いわで援軍の意味がないので補強する
            // 同格の物理攻撃は全く当たらないので物理防御の補強の必要なし　やっぱ物理はクソバランス
            // 使えそうなら持ってっていいよ
            chara.elements.ModBase(SKILL.DMG, 10);
            chara.elements.ModBase(SKILL.penetration, 25);
            chara.elements.ModBase(SKILL.antiMagic, 30);
            chara.elements.ModBase(ENC.mod_chaser, 20);

            return chara;
        }

        private Chara CreateGuard()
        {
            var @quest = this.quest as QuestRoadDefence;
            int charaLv = quest.dangerLv;
            var chara = CharaGen.Create((@quest.IsMofuVillage) ? "guard_fox" : "guard", charaLv);
            chara.SetLv(charaLv);
            chara.hostility = Hostility.Friend;
            chara.c_originalHostility = Hostility.Friend;

            // これも命中と火力と耐久を補強
            chara.elements.ModBase(SKILL.DMG, 10);
            chara.elements.ModBase(SKILL.penetration, 25);
            chara.elements.ModBase(SKILL.antiMagic, 60);
            chara.elements.ModBase(ENC.mod_chaser, 20);
            chara.elements.ModBase(ENC.mod_frustration, 25);

            return chara;
        }

        private void CacheCandEnemies()
        {
            var @quest = this.quest as QuestRoadDefence;
            _CandEnemiesCache = EClass.sources.charas.rows.Where(
                r => r.race == @quest.TargetRaceId &&
                r.quality == 0 &&
                r.chance > 0 &&
                r.hostility == ""/*Enemy*/ &&
                r.LV <= quest.dangerLv &&
                !r.actCombat.Where(s => s.StartsWith("ActEscape")).Any()
            ).ToDictionary(r => r.id, r => r.chance);
            _CandChanceSum = _CandEnemiesCache.Sum(kvp => kvp.Value);
        }
    }
}
