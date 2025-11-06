using System.Collections.Generic;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventWGDefence : ZoneEventUrbanDefence
    {
        internal override HashSet<string> SpawnCandidateList { get; } = new HashSet<string>
        {
            "dog_hound",
            "rock_thrower",
            "rogue",
            "hardgay",
            "guild_thief",
            "merc_mage",
            "merc_warrior",
            "silvereye",
            "flower_girl", // メテオぶっぱウーマンを呼ぶのは色々崩れそう
            "lion", // ダルフィにいたから呼んできましたシリーズその1
            "giant", // ダルフィにいたから呼んできましたシリーズその2
        };

        internal override void SpawnEnemies(int dangerLv, int numEnemies = 1)
        {
            // 階段が2箇所あるのでどっちかから出現させる
            // ギルドは地下にあるので登り階段をとればいい
            var spawnPointList = new List<Point>();
            foreach (var thing in EClass._map.ListThing<TraitStairsUp>())
            {
                spawnPointList.Add(thing.pos);
            }

            for (int i = 0; i < numEnemies; i++)
            {
                Point boundaryEdgePoint = spawnPointList.RandomItem().GetNearestPoint(allowBlock: false, allowChara: false, minRadius: 1);
                Chara enemy = CreateEnemy(dangerLv);
                EClass._zone.AddCard(enemy, boundaryEdgePoint);
                enemies.Add(enemy.uid);
            }
        }
    }
}
