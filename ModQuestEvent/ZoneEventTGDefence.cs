using System.Collections.Generic;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventTGDefence : ZoneEventUrbanDefence
    {
        // ギルド構成員、傭兵、戦士っぽい人型モンスター
        internal override HashSet<string> SpawnCandidateList { get; } = new HashSet<string>
        {
            "centaur_archer",
            "merc",
            "infantry",
            "skeleton_warrior",
            "skeleton_berserker",
            "guild_warrior",
            "samurai_kamikaze",
            "shamo",
            "dog_shiba",
            "quickling_archer",
            "minotaur_fighter",
            "merc_archer",
            "merc_mage",
            "merc_warrior",
            "silvereye",
        };
        public override void OnVisit()
        {
            base.OnVisit();
            if (!EClass.game.isLoading)
            {
                EClass._zone.parent = ThiefGuildZone;
            }
        }

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
