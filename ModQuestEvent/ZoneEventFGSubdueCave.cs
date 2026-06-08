using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventFGSubdueCave : ZoneEventSubdue
    {
        public override void OnVisit()
        {
            if (!EClass.game.isLoading)
            {
                EClass._zone._dangerLv = CalcDangerLv();
                MoveToEnterPosition();
                EClass._map.RevealAll();
                SummonReinforcements();
                Spawn(4 + base.quest.difficulty * 2 + EClass.rnd(5));
                SpawnBoss(1);
                EClass._zone.SetBGM(110);
                max = enemies.Count;
            }
        }

        internal virtual int CalcDangerLv()
        {
            return Mathf.Max(base.quest.DangerLv - 2, 1);
        }

        internal void MoveToEnterPosition() 
        {
            var enterPos = new Point(EClass._map.bounds.CenterX, EClass._map.bounds.z + 2);

            foreach (var chara in EClass._map.charas)
            {
                if (!chara.HasHost)
                {
                    chara._Move(enterPos.GetNearestPoint(allowBlock: false, allowChara: false));
                    chara.renderer.SetFirst(first: true, chara.pos.PositionCenter());
                }
            }

            EClass.screen.FocusPC();
            EClass.screen.RefreshPosition();
        }

        public new void Spawn(int num = 1)
        {
            int north = (EClass._map.bounds.z + EClass._map.bounds.maxZ) * 3 / 4;
            int south = (EClass._map.bounds.z + EClass._map.bounds.maxZ) * 1 / 4;
            int west = (EClass._map.bounds.x + EClass._map.bounds.maxX) * 1 / 4;
            int east = (EClass._map.bounds.x + EClass._map.bounds.maxX) * 3 / 4;
            int centerX = EClass._map.bounds.CenterX;
            int centerZ = EClass._map.bounds.CenterZ;

            var spawnDirection = new List<Point>
            {
                new Point(centerX, north),
                new Point(east, north),
                new Point(east, centerZ),
                new Point(east, south),
                //new Point(centerX, south),
                new Point(west, south),
                new Point(west, centerZ),
                new Point(west, north),
                new Point(centerX, centerZ),
            };

            for (int i = 0; i < num; i++)
            {
                var spawnPoint = spawnDirection.RandomItem().GetNearestPoint(allowBlock: false, allowChara: false);
                var enemy = CreateEnemy(CalcDangerLv(), spawnPoint);
                EClass._zone.AddCard(enemy, spawnPoint);

                if (CountEnemy)
                {
                    enemies.Add(enemy.uid);
                }
            }
        }

        public void SpawnBoss(int num = 1)
        {
            int north = (EClass._map.bounds.z + EClass._map.bounds.maxZ) * 3 / 4;
            int south = (EClass._map.bounds.z + EClass._map.bounds.maxZ) * 1 / 4;
            int west = (EClass._map.bounds.x + EClass._map.bounds.maxX) * 1 / 4;
            int east = (EClass._map.bounds.x + EClass._map.bounds.maxX) * 3 / 4;
            int centerX = EClass._map.bounds.CenterX;
            int centerZ = EClass._map.bounds.CenterZ;

            var spawnDirection = new List<Point>
            {
                new Point(centerX, north),
                new Point(east, north),
                new Point(east, centerZ),
                new Point(east, south),
                //new Point(centerX, south),
                new Point(west, south),
                new Point(west, centerZ),
                new Point(west, north),
                new Point(centerX, centerZ),
            };

            for (int i = 0; i < num; i++)
            {
                var spawnPoint = spawnDirection.RandomItem().GetNearestPoint(allowBlock: false, allowChara: false);
                var enemy = CreateEnemy(CalcDangerLv(), spawnPoint, Rarity.Legendary);
                EClass._zone.AddCard(enemy, spawnPoint);

                if (CountEnemy)
                {
                    enemies.Add(enemy.uid);
                }
            }
        }

        internal virtual Chara CreateEnemy(int dangerLv, in Point pos, Rarity rarity = Rarity.Normal)
        {
            var spawnList = GetSpawnListByBiome(pos.cell.biome);

            var spawnCharaSource = spawnList.Select(lv: dangerLv);
            var charaRarity = rarity;
            int charaLv = (spawnCharaSource.LV + ((dangerLv >= 51) ? 50 : 0)) * Mathf.Max(1, (dangerLv - 1) / 50);
            charaLv = (rarity == Rarity.Legendary) ? charaLv * 3 / 2 : charaLv;


            CardBlueprint.Set(new CardBlueprint
            {
                rarity = charaRarity,
                lv = charaLv,
            });

            var createdChara = CharaGen.Create(spawnCharaSource.id);
            createdChara.c_originalHostility = Hostility.Enemy;
            createdChara.hostility = Hostility.Enemy;

            if (rarity == Rarity.Legendary)
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

        private void SummonReinforcements()
        {
            for (int i = 0; i < 3; i++)
            {
                var chara = CharaGen.Create("guild_warrior", CalcDangerLv());
                chara.SetHomeZone(EClass._zone);
                chara.MakeMinion(EClass.pc);
                EClass._zone.AddCard(chara, EClass.pc.pos.GetNearestPoint(allowChara: false, minRadius: 2));
            }
        }
    }
}
