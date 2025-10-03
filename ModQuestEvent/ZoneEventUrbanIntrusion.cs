using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventUrbanIntrusion : ZoneEventSubdue
    {
        public override void OnVisit()
        {
            if (!EClass.game.isLoading)
            {
                RemoveAllInhabitants();
                RemoveAllMedals();
                RemoveAllStairs();
                EClass._map.RevealAll();

                int spawnNumbers = 10 + EClass.rndHalf(base.quest.difficulty * 3);
                EClass._zone._dangerLv = Mathf.Max(base.quest.DangerLv - 2, 1);
                SpawnEnemies(spawnNumbers);
                AggroEnemy(15);
                EClass._zone.SetBGM(102);
                max = enemies.Count;
            }
        }

        private void SpawnEnemies(int num = 1)
        {
            int mapWidth = EClass._map.Width;
            int mapHeight = EClass._map.Height;

            // 初期位置近辺には沸かせない
            // 端に近すぎると進入困難な外壁に埋まる(主にパルミアとダルフィのせい)
            // 8箇所から沸かせたいが凄く遠くなって面倒くさい(主にパルミアとダルフィのせい)
            var northPoint = new Point(mapWidth * 2 / 4, mapHeight * 3 / 4);
            var southPoint = new Point(mapWidth * 2 / 4, mapHeight * 1 / 4);
            var westPoint = new Point(mapWidth * 1 / 4, mapHeight * 2 / 4);
            var eastPoint = new Point(mapWidth * 3 / 4, mapHeight * 2 / 4);
            var spawnDirection = new List<Point> { northPoint, southPoint, westPoint, eastPoint };

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

                Chara chara = EClass._zone.SpawnMob(spawnPoint, SpawnSetting.DefenseEnemy(EClass._zone.DangerLv));

                Hostility hostility2 = (chara.c_originalHostility = Hostility.Enemy);
                chara.hostility = hostility2;
                if (CountEnemy)
                {
                    enemies.Add(chara.uid);
                }
            }
        }

        // ポカやらかしやすいからプログラム側でも消す
        private void RemoveAllInhabitants()
        {
            List<int> shouldDieUID = new List<int>();
            foreach (var chara in EClass._map.charas)
            {
                if(chara != null && !chara.IsPCFaction)
                {
                    shouldDieUID.Add(chara.uid);
                }
            }

            foreach (var UID in shouldDieUID)
            {
                EClass._map.charas.Where(c => c.uid == UID).First().Destroy();
            }
        }

        private void RemoveAllMedals()
        {
            List<int> shouldDieUID = new List<int>();
            foreach (var thing in EClass._map.things)
            {
                if (thing != null && thing.id == "medal")
                {
                    shouldDieUID.Add(thing.uid);
                }
            }

            foreach (var UID in shouldDieUID)
            {
                EClass._map.things.Where(c => c.uid == UID).First().Destroy();
            }
        }

        private void RemoveAllStairs()
        {
            List<int> shouldDieUID = new List<int>();
            foreach (var thing in EClass._map.things)
            {
                if (thing != null && (thing.source.trait.Contains("StairsDown") || thing.source.trait.Contains("StairsUp")))
                {
                    shouldDieUID.Add(thing.uid);
                }
            }

            foreach (var UID in shouldDieUID)
            {
                EClass._map.things.Where(c => c.uid == UID).First().Destroy();
            }
        }
    }
}
