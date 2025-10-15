using System.Linq;
using UnityEngine;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventFGSubdue : ZoneEventSubdue
    {
        public override void OnVisit()
        {
            if (!EClass.game.isLoading)
            {
                EClass._zone._dangerLv = CalcDangerLv();
                Spawn(4 + base.quest.difficulty * 2 + EClass.rnd(5));
                AggroEnemy(15);
                EClass._zone.SetBGM(102);
                max = enemies.Count;
                EClass._zone.parent = FighterGuildZone;
            }
        }

        internal virtual int CalcDangerLv()
        {
            return Mathf.Max(base.quest.DangerLv - 2, 1);
        }

        public new void Spawn(int num = 1)
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
