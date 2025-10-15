using RandomQuestExpantion.ModQuests.Common;
using System.Collections.Generic;
using System.Linq;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventFarmFieldWar : ZoneEventDefenseGame
    {
        public override string TextWidgetDate => "byakko_mod_status_farmfield_war".lang(wave.ToString() ?? "",  EClass._zone.DangerLv.ToString() ?? "", kills.ToString() ?? "", KillRequirements.ToString()) + ((instance != null && retreated) ? "defenseRetreating".lang() : "");

        public int KillRequirements => 30 + 10 * base.quest.difficulty;

        public int InitialDangerLV => 1; // 1 + 1

        public override void OnVisit()
        {
            if (EClass.game.isLoading)
            {
                QuestFarmFieldWar.lastWave = wave;
                QuestFarmFieldWar.bonus = bonus;
                return;
            }
            EClass._zone._dangerLv = InitialDangerLV;

            SetFarmFieldPiece();

            EClass._zone.SetBGM(107);

            Msg.Say("defense_start");
            NextWave();
        }

        private void SetFarmFieldPiece()
        {
            GenBounds genBounds = GenBounds.Create(EClass._zone);
            genBounds.marginPartial = 2;
            genBounds.FuncCheckEmpty = (Cell cell) => cell.sourceFloor.id == 42;

            List<SourceObj.Row> crops = EClass.sources.objs.rows.Where((SourceObj.Row o) => o.tag.Contains("harvest")).ToList();
            for (int i = 0; i < 50; i++)
            {
                genBounds.TryAddMapPiece(MapPiece.Type.Farm, 0f, "", delegate (PartialMap p, GenBounds b)
                {
                    List<Point> list = b.ListEmptyPoint();
                    SourceObj.Row row = crops.RandomItemWeighted((SourceObj.Row o) => o.chance);

                    foreach (Point item in list)
                    {
                        if (item.sourceFloor.id == 4)
                        {
                            item.SetObj(row.id);
                            item.growth.SetStage(item.growth.HarvestStage - 1);
                            item.cell.isClearSnow = true;
                        }
                    }
                });
            }
            foreach (Thing thing in EClass._map.things)
            {
                thing.isNPCProperty = true;
            }
        }

        public override void _OnTickRound()
        {
            QuestFarmFieldWar.lastWave = wave;
            QuestFarmFieldWar.bonus = bonus;
            turns++;

            if (hornTimer > 0)
            {
                hornTimer--;
            }

            if (turns <= 3 + base.quest.difficulty)
            {
                SpawnAnimal(3);
            }

            if (turns == 7 && wave % 10 == 0)
            {
                Rand.SetSeed(wave + base.quest.uid);
                SpawnBossAnimal();
                Rand.SetSeed();
            }

            if (turns == 12 - (base.quest.difficulty / 2))
            {
                NextWave();
            }

            AggroEnemy();
        }

        private void NextWave()
        {
            wave++;
            turns = 0;
            EClass._zone._dangerLv += 1;
            SE.Play("warhorn");
            Msg.Say("warhorn");
            Msg.Say("defense_wave", wave.ToString() ?? "", EClass._zone.DangerLv.ToString() ?? "");
            SpawnAnimal(2 + base.quest.difficulty);
            AggroEnemy();
        }

        private void SpawnAnimal(int num = 1)
        {
            for (int i = 0; i < num; i++)
            {
                Point boundaryEdgePoint = EClass._map.bounds.GetRandomEdge().GetNearestPoint(allowBlock: false, allowChara: false, minRadius: 3);
                Chara spawnedChara = EClass._zone.SpawnMob(boundaryEdgePoint, SpawnSetting.HomeWild(EClass._zone.DangerLv));
                spawnedChara.c_originalHostility = Hostility.Enemy;
                spawnedChara.hostility = Hostility.Enemy;
            }
        }

        private void SpawnBossAnimal()
        {
            Point boundaryEdgePoint = EClass._map.bounds.GetRandomEdge().GetNearestPoint(allowBlock: false, allowChara: false, minRadius: 3);
            var bossSpawnSetting = SpawnSetting.Boss(EClass._zone.DangerLv);
            bossSpawnSetting.idSpawnList = "c_animal";
            Chara chara = EClass._zone.SpawnMob(boundaryEdgePoint, bossSpawnSetting);
            Hostility hostility2 = (chara.c_originalHostility = Hostility.Enemy);
            chara.hostility = hostility2;

            Msg.Say("defense_boss", chara.Name);
            EClass.game.Pause();
        }

        public override void OnCharaDie(Chara c)
        {
            if (c.IsPCParty || c.IsPCPartyMinion)
            {
                return;
            }

            kills++;
            bonus += CalcBonusMoney(c);

            if (!retreated && instance.status != ZoneInstance.Status.Success && kills >= KillRequirements)
            {
                SE.Play("warhorn");
                Msg.Say("warhorn");
                Msg.Say("defense_retreat");
                retreated = true;
                instance.status = ZoneInstance.Status.Success;
                ActEffect.Proc(EffectId.Evac, EClass.pc);
            }
        }

        private int CalcBonusMoney(in Chara killedChara)
        {
            int baseMoney = killedChara.LV * 4;
            return EClass.rndHalf(baseMoney);
        }
    }
}
