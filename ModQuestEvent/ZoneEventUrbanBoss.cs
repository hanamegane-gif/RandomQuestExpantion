using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventUrbanBoss : ZoneEventSubdue
    {        
        public int WaveIntervalRound => 6; // TickRoundはだいたい3～4Tに一度

        public int MaxWaves => base.quest.difficulty * 2 / 7 + 2; // 1～3: 2, 4～6:3, 7:4

        [JsonProperty]
        public int NextWaveRoundRemains = 4; // 最初の出現まで4 * 3.5Tのインターバル

        [JsonProperty]
        public int CurrentWave = 0;

        public bool IsInInterval => NextWaveRoundRemains > 0 && IsRemainsNextWave && !IsEngaging;

        public bool IsCompletable => !IsRemainsNextWave && !IsEngaging;

        public bool IsQuestFinished => EClass._zone.instance.status == ZoneInstance.Status.Success || EClass._zone.instance.status == ZoneInstance.Status.Fail;

        public bool IsEngaging => enemies.Any();

        public bool IsRemainsNextWave => CurrentWave < MaxWaves;

        public int RemainWaves => MaxWaves - CurrentWave;

        public bool ShouldTriggerNextWave => !IsEngaging && IsRemainsNextWave && NextWaveRoundRemains <= 0;

        public Chara CurrentBoss => EClass._map.charas.Find(c => c.uid == enemies.FirstOrDefault());

        public override void OnVisit()
        {
            if (!EClass.game.isLoading)
            {
                RemoveAllInhabitants();
                RemoveAllMedals();
                RemoveAllStairs();

                EClass._zone._dangerLv = Mathf.Max(base.quest.DangerLv - 2, 1);

                SummonReinforcements();
                EClass._map.RevealAll();
                EClass._zone.SetBGM(101);
            }
        }

        public override void OnCharaDie(Chara c)
        {
            if (c != null && enemies.Any(uid => uid ==  c.uid))
            {
                enemies.Remove(c.uid);

                NextInterval();
                CheckClear();
            }
        }

        public override void _OnTickRound()
        {
            if (IsInInterval)
            {
                NextWaveRoundRemains--;
            }
            else if (ShouldTriggerNextWave)
            {
                NextWave();
            }
            else
            {
                AggroEnemy();
            }
        }

        private void NextWave()
        {
            SpawnBoss();
            AggroEnemy(15);
            max = enemies.Count;
        }

        private void NextInterval()
        {
            NextWaveRoundRemains = WaveIntervalRound;
            CurrentWave++;
        }

        private new void CheckClear()
        {
            if (IsQuestFinished)
            {
                return;
            }
            if (IsCompletable)
            {
                EClass._zone.instance.status = ZoneInstance.Status.Success;
                Msg.Say("subdue_complete");
                EClass._zone.SetBGM();
                SE.Play("Jingle/fanfare");
            }
        }

        private void SpawnBoss()
        {
            int mapWidth = EClass._map.Width;
            int mapHeight = EClass._map.Height;

            // 初期位置近辺には沸かせない
            // 端に近すぎると進入困難な外壁に埋まる(主にパルミアとダルフィのせい)
            // 8箇所から沸かせたいが凄く遠くなって面倒くさい(主にパルミアとダルフィのせい)
            Point spawnPoint = null;
            var northPoint = new Point(mapWidth * 2 / 4, mapHeight * 3 / 4);
            var southPoint = new Point(mapWidth * 2 / 4, mapHeight * 1 / 4);
            var westPoint = new Point(mapWidth * 1 / 4, mapHeight * 2 / 4);
            var eastPoint = new Point(mapWidth * 3 / 4, mapHeight * 2 / 4);
            var spawnDirection = new List<Point> { northPoint, southPoint, westPoint, eastPoint };

            // 今の所斜めから進入するマップはないので無限ループでも大丈夫だと思われる
            while (spawnPoint == null)
            {
                spawnPoint = spawnDirection.RandomItem().GetRandomPoint(allowBlocked: false, allowChara: true, radius: 20);

                // 水場に沸かせない(主にポトカとルミエストのせい)
                if (spawnPoint != null && spawnPoint.cell.IsDeepWater)
                {
                    spawnPoint = null;
                }
            }

            Chara chara = EClass._zone.TryGenerateEvolved(force: true, spawnPoint);
            chara.c_originalHostility = Hostility.Enemy;
            chara.hostility = Hostility.Enemy;
            chara.bossText = true;

            enemies.Add(chara.uid);

            // ｶｶｯはあるが停止しないといきなり出てきた感がすごかった
            Msg.Say("defense_boss", chara.Name);
            EClass.game.Pause();
        }

        private void SummonReinforcements()
        {
            ActEffect.ProcAt(EffectId.Summon, 100, BlessedState.Normal, EClass.pc, EClass.pc, EClass.pc.pos, isNeg: false, new ActRef
            {
                n1 = "special_force"
            });
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
