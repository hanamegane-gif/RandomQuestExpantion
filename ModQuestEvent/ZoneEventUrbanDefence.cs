using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventUrbanDefence : ZoneEventSubdue
    {
        public int MaxWaves => base.quest.difficulty * 2 / 7 + 5; // 1～3: 5, 4～6:6, 7:7 ★7依頼なんてほぼ出ないけど

        [JsonProperty]
        public int NextWaveRoundRemains = 0;

        [JsonProperty]
        public int CurrentWave = 0;

        public int RoundUntilNextWave => 10; // 次Waveまで10 * 3.5Tの間隔

        public bool IsCompletable => !IsRemainsNextWave && !IsEngaging;

        public bool IsQuestFinished => EClass._zone.instance.status == ZoneInstance.Status.Success || EClass._zone.instance.status == ZoneInstance.Status.Fail;

        public bool IsEngaging => enemies.Any();

        public bool IsRemainsNextWave => CurrentWave < MaxWaves;

        public int RemainWaves => MaxWaves - CurrentWave;

        public bool ShouldTriggerNextWave => IsRemainsNextWave && (CondiTooLate || CondiLowEnemy || CondiWaveSkip);

        public bool CondiWaveSkip => !IsEngaging;

        public bool CondiLowEnemy => NextWaveRoundRemains <= 0 && enemies.Count <= 3;

        public bool CondiTooLate => NextWaveRoundRemains <= -20;

        // クエストに登場させる敵を選ぶ仕組み
        // 街で出されるクエストではないので適当なの置いておく
        internal virtual HashSet<string> SpawnCandidateList { get; } = new HashSet<string>
        {
            "jure",
        };

        public override void OnVisit()
        {
            if (!EClass.game.isLoading)
            {
                RemoveAllInhabitants(EClass._map);
                RemoveAllMedals(EClass._map);
                EClass._map.RevealAll();

                int dangerLv = CalcZoneDangerLv();
                EClass._zone._dangerLv = dangerLv;
                EClass._zone.SetBGM(110);
                Msg.Say("defense_start");

                NextWave();
            }
        }

        internal virtual int CalcZoneDangerLv()
        {
            return Mathf.Max(base.quest.DangerLv - 2, 1);
        }

        public override void _OnTickRound()
        {
            NextWaveRoundRemains--;
            CheckClear();
            if (ShouldTriggerNextWave)
            {
                NextWave();
            }
            AggroEnemy();
        }

        public override void OnCharaDie(Chara c)
        {
            CheckClear();
            if (ShouldTriggerNextWave)
            {
                NextWave();
            }
        }

        public new void CheckClear()
        {
            if (EClass._zone.instance.status == ZoneInstance.Status.Success || EClass._zone.instance.status == ZoneInstance.Status.Fail)
            {
                return;
            }

            enemies.ForeachReverse(delegate (int id)
            {
                Chara chara = EClass._map.FindChara(id);
                if (chara == null || !chara.IsAliveInCurrentZone || !EClass.pc.IsHostile(chara))
                {
                    enemies.Remove(id);
                }
            });
            if (IsCompletable)
            {
                EClass._zone.instance.status = ZoneInstance.Status.Success;
                Msg.Say("subdue_complete");
                EClass._zone.SetBGM();
                SE.Play("Jingle/fanfare");
            }
        }

        internal virtual void NextWave()
        {
            CurrentWave++;
            NextWaveRoundRemains = RoundUntilNextWave;

            SE.Play("warhorn");
            Msg.Say("warhorn");
            Msg.Say("defense_wave", CurrentWave.ToString() ?? "", CalcZoneDangerLv().ToString() ?? "");
            SpawnEnemies(EClass._zone._dangerLv, CalcNumberOfEnemies());
            AggroEnemy(15);
        }

        internal virtual int CalcNumberOfEnemies()
        {
            return 8 + EClass.rnd(3 + base.quest.difficulty);
        }

        internal virtual void SpawnEnemies(int dangerLv, int numEnemies = 1)
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

            for (int i = 0; i < numEnemies; i++)
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

                Chara enemy = CreateEnemy(dangerLv);
                enemy.c_originalHostility = Hostility.Enemy;
                enemy.hostility = Hostility.Enemy;
                EClass._zone.AddCard(enemy, spawnPoint);

                if (CountEnemy)
                {
                    enemies.Add(enemy.uid);
                }
            }
        }

        internal virtual Chara CreateEnemy(int dangerLv)
        {
            // 出現数を多くする分Lvは控えめにする
            int generateLv = Mathf.Max((dangerLv) * 3 / 4, 5);
            var spawnCharaSource = SpawnListChara.Get("all", (SourceChara.Row r) => SpawnCandidateList.Contains(r.id)).Select(lv: generateLv);
            int charaLv = (spawnCharaSource.LV + ((generateLv >= 51) ? 50 : 0)) * Mathf.Max(1, (generateLv - 1) / 50);

            CardBlueprint cardBlueprint = new CardBlueprint
            {
                lv = charaLv,
            };
            CardBlueprint.Set(cardBlueprint);

            Chara createdChara = CharaGen.Create(spawnCharaSource.id);
            createdChara.c_originalHostility = Hostility.Enemy;
            createdChara.hostility = Hostility.Enemy;

            return createdChara;
        }
    }
}
