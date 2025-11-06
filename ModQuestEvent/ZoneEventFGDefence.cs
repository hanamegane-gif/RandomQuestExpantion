using System.Collections.Generic;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventFGDefence : ZoneEventUrbanDefence
    {
        // 魔法属性持ち共は出禁です！！！！
        internal override HashSet<string> SpawnCandidateList { get; } = new HashSet<string>
        {
            "mage_app",
            "imp_nether",
            "imp_chaos",
            "mage",
            "puppet",
            "fairy",
            "golem_stone",
            "skeleton_spartoi",
            "missionary_dark",
            "sahagin_sorcerer",
            "hand_magic",
            "merc_archer",
            "merc_mage",
            "merc_warrior",
            "scholar", // 特に戦闘能力はないけど呼ばれたシリーズその1
        };

        internal override void SpawnEnemies(int dangerLv, int numEnemies = 1)
        {
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
