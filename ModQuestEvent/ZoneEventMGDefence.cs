using System.Collections.Generic;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventMGDefence : ZoneEventUrbanDefence
    {
        // ギルド構成員だけを出すと簡単すぎる(あと面白くない)のでそれっぽいのを混ぜる
        // ギルド構成員、傭兵、盗賊っぽい人型モンスター、飼いならしやすそうなモンスターや動物
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
        public override void OnVisit()
        {
            base.OnVisit();
            if (!EClass.game.isLoading)
            {
                EClass._zone.parent = MerchantGuildZone;
            }
        }

        internal override void SpawnEnemies(int dangerLv, int numEnemies = 1)
        {
            for (int i = 0; i < numEnemies; i++)
            {
                // 商人ギルドは階段がなくマップ端から進入する作りになっている
                Point boundaryEdgePoint = EClass._map.bounds.GetRandomEdge().GetNearestPoint(allowBlock: false, allowChara: false, minRadius: 2);
                Chara enemy = CreateEnemy(dangerLv);
                EClass._zone.AddCard(enemy, boundaryEdgePoint);
                enemies.Add(enemy.uid);
            }
        }
    }
}
