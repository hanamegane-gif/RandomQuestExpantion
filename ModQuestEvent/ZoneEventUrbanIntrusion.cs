using System.Collections.Generic;
using UnityEngine;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventUrbanIntrusion : ZoneEventSubdue
    {
        public override void OnVisit()
        {
            if (!EClass.game.isLoading)
            {
                RemoveAllInhabitants(EClass._map);
                RemoveAllMedals(EClass._map);
                RemoveAllStairs(EClass._map);
                EClass._map.RevealAll();

                int dangerLv = CalcZoneDangerLv();
                EClass._zone._dangerLv = dangerLv;
                SpawnEnemies(dangerLv, CalcNumberOfEnemies());
                AggroEnemy(15);
                EClass._zone.SetBGM(98);
                max = enemies.Count;
            }
        }

        internal virtual int CalcZoneDangerLv()
        {
            return Mathf.Max(base.quest.DangerLv - 2, 1);
        }

        internal virtual int CalcNumberOfEnemies()
        {
            return 10 + EClass.rndHalf(base.quest.difficulty * 3);
        }

        internal virtual void SpawnEnemies(int dangerLv, int num = 1)
        {
            // 初期位置近辺には沸かせない
            // 端に近すぎると進入困難な外壁に埋まる(主にパルミアとダルフィのせい)
            // 8箇所から沸かせたいが凄く遠くなって面倒くさい(主にパルミアとダルフィのせい)
            int north = (EClass._map.bounds.z + EClass._map.bounds.maxZ) * 3 / 4;
            int south = (EClass._map.bounds.z + EClass._map.bounds.maxZ) * 1 / 4;
            int west = (EClass._map.bounds.x + EClass._map.bounds.maxX) * 1 / 4;
            int east = (EClass._map.bounds.x + EClass._map.bounds.maxX) * 3 / 4;
            int centerX = EClass._map.bounds.CenterX;
            int centerZ = EClass._map.bounds.CenterZ;

            var spawnDirection = new List<Point>
            {
                new Point(centerX, north),
                new Point(east, centerZ),
                new Point(centerX, south),
                new Point(west, centerZ),
            };
            for (int i = 0; i < num; i++)
            {
                Point spawnPoint = null;
                for (int j = 0; j < 100; j++)
                {
                    spawnPoint = spawnDirection.RandomItem().GetRandomPoint(allowBlocked: false, allowChara: true, radius: 20);

                    // 水場に沸かせない(主にポトカとルミエストのせい)
                    if (spawnPoint != null && !spawnPoint.cell.IsDeepWater)
                    {
                        break;
                    }
                }

                if (spawnPoint == null)
                {
                    continue;
                }

                Chara enemy = EClass._zone.SpawnMob(spawnPoint, SpawnSetting.DefenseEnemy(dangerLv));

                enemy.c_originalHostility = Hostility.Enemy;
                enemy.hostility = Hostility.Enemy;
                if (CountEnemy)
                {
                    enemies.Add(enemy.uid);
                }
            }
        }
    }
}
