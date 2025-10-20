using System.Linq;
using UnityEngine;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventFGWithdrawal : ZoneEventDefenseGame
    {
        public override string TextWidgetDate => "defenseWave".lang(wave.ToString() ?? "", kills.ToString() ?? "", CalcDangerLv().ToString() ?? "") + ((instance != null && retreated) ? "defenseRetreating" : "").lang();

        public new bool CanRetreat => wave >= CanRetreatWave;
        
        // 10Waveだときつすぎ5Waveだとゆるい
        public int CanRetreatWave => 7;

        public override void OnVisit()
        {
            if (EClass.game.isLoading)
            {
                QuestDefenseGame.lastWave = wave;
                QuestDefenseGame.bonus = bonus;
                return;
            }

            int dangerLv = CalcDangerLv();
            EClass._zone._dangerLv = dangerLv + 5;
            bonus += dangerLv / 10;
            EClass._zone.SetBGM(107);
            Msg.Say("defense_start");
            NextWave();

            // これをやると現在のマップの危険度が壊れることがわかった
            // だからといってこの処理を無くすと戦争マップから離脱できない
            EClass._zone.parent = FighterGuildZone;
        }

        internal virtual int CalcDangerLv()
        { 
            return EClass.pc.FameLv * 75 / 100 + 5;
        }

        public new void NextWave(int add = 0)
        {
            wave++;
            turns = 0;
            SE.Play("warhorn");
            Msg.Say("warhorn");
            Msg.Say("defense_wave", wave.ToString() ?? "", CalcDangerLv().ToString() ?? "");
            Spawn(2 + base.quest.difficulty + add);
            AggroEnemy();

            if (CanRetreat && !retreated)
            {
                Retreat();
            }
        }

        public override void _OnTickRound()
        {
            QuestDefenseGame.lastWave = wave;
            QuestDefenseGame.bonus = bonus;
            turns++;

            if (turns <= 5 + base.quest.difficulty)
            {
                Spawn(1);
            }

            if (turns == 5 && wave % 3 == 0)
            {
                Rand.SetSeed(wave + base.quest.uid);
                SpawnBoss(EClass.rnd(2) == 0);
                Rand.SetSeed();
            }

            if (turns == 10)
            {
                NextWave();
            }

            AggroEnemy();
        }

        internal new void Spawn(int num = 1)
        {
            for (int i = 0; i < num; i++)
            {
                Point spawnPoint = EClass._map.bounds.GetRandomEdge().GetNearestPoint(allowBlock: false, allowChara: false);
                Chara enemy = CreateEnemy(CalcDangerLv(), spawnPoint);
                EClass._zone.AddCard(enemy, spawnPoint);
                if (CountEnemy)
                {
                    enemies.Add(enemy.uid);
                }
            }
        }

        internal new void SpawnBoss(bool evolve = false)
        {
            Point spawnPoint = EClass._map.bounds.GetRandomEdge().GetNearestPoint(allowBlock: false, allowChara: false);
            Chara enemy = CreateEnemy(CalcDangerLv(), spawnPoint, rarity: Rarity.Legendary, evolved: EClass.rnd(2) == 0);
            EClass._zone.AddCard(enemy, spawnPoint);
            if (CountEnemy)
            {
                enemies.Add(enemy.uid);
            }

            if (WarnBoss)
            {
                Msg.Say("defense_boss", enemy.Name);
                EClass.game.Pause();
            }
        }

        private void Retreat()
        {
            Msg.Say("defense_retreat");
            retreated = true;
            instance.status = ZoneInstance.Status.Success;
            ActEffect.Proc(EffectId.Evac, EClass.pc);
        }

        internal virtual Chara CreateEnemy(int dangerLv, in Point pos, Rarity rarity = Rarity.Normal, bool evolved = false)
        {
            SpawnList spawnList = GetSpawnListByBiome(pos.cell.biome);

            var spawnCharaSource = spawnList.Select(lv: dangerLv);
            var charaRarity = (evolved) ? Rarity.Legendary : rarity;
            int charaLv = (spawnCharaSource.LV + ((dangerLv >= 51) ? 50 : 0)) * Mathf.Max(1, (dangerLv - 1) / 50);
            charaLv = (evolved) ? charaLv * 2 + 20 :
                      (rarity == Rarity.Legendary) ? charaLv * 3 / 2 : charaLv;

            CardBlueprint.Set(new CardBlueprint
            {
                rarity = charaRarity,
                lv = charaLv,
            });

            Chara createdChara = CharaGen.Create(spawnCharaSource.id);
            createdChara.c_originalHostility = Hostility.Enemy;
            createdChara.hostility = Hostility.Enemy;

            if (evolved)
            {
                createdChara.c_bossType = BossType.Evolved;
                CreateIdentity(createdChara);
            }
            else if (rarity == Rarity.Legendary)
            {
                createdChara.c_bossType = BossType.Boss;
            }

            createdChara.c_originalHostility = Hostility.Enemy;
            createdChara.hostility = Hostility.Enemy;


            return createdChara;
        }

        internal SpawnList GetSpawnListByBiome(in BiomeProfile b)
        {
            var biome = b;
            SpawnList spawnList = null;
            spawnList = (!(biome.spawn.chara.Any()) ? SpawnList.Get(biome.name, "chara", new CharaFilter
            {
                ShouldPass = delegate (SourceChara.Row s)
                {
                    if (s.hostility != "")
                    {
                        return false;
                    }

                    return s.biome == biome.name || s.biome.IsEmpty();
                }
            }) : SpawnList.Get(biome.spawn.GetRandomCharaId()));

            return spawnList;
        }

        internal void CreateIdentity(Chara evolvedChara)
        {
            for (int i = 0; i < 2 + EClass.rnd(2); i++)
            {
                evolvedChara.ability.AddRandom();
            }

            evolvedChara.AddThing(evolvedChara.MakeGene(DNA.Type.Default));
            if (EClass.rnd(2) == 0)
            {
                evolvedChara.AddThing(evolvedChara.MakeGene(DNA.Type.Superior));
            }
        }
    }
}
