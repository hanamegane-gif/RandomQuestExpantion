using UnityEngine;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventSubdueGhost : ZoneEventSubdue
    {
        public override void OnVisit()
        {
            if (!EClass.game.isLoading)
            {
                int dangerLv = CalcZoneDangerLv();
                int numEnemies = CalcNumberOfEnemies();
                EClass._zone._dangerLv = dangerLv;

                SetConcertHallPiece();

                EClass._zone.AddCard(ThingGen.Create("376"), EClass.pc.pos.GetNearestPoint()).Install().isNPCProperty = true;

                EClass._map.RevealAll();
                SpawnEnemies(dangerLv, numEnemies);
                AggroEnemy(15);

                EClass._zone.SetBGM(102);
                max = enemies.Count;
            }
        }

        public override void OnCharaDie(Chara c)
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
            if (enemies.Count == 0)
            {
                EClass._zone.instance.status = ZoneInstance.Status.Success;
                Msg.Say("subdue_complete");
                EClass._zone.SetBGM();
                SE.Play("Jingle/fanfare");
            }
        }

        internal virtual int CalcZoneDangerLv()
        {
            return Mathf.Max(base.quest.DangerLv - 2, 1);
        }

        internal virtual int CalcNumberOfEnemies()
        {
            return 4 + base.quest.difficulty * 2 + EClass.rndHalf(6);
        }


        internal virtual void SpawnEnemies(int dangerLv, int numEnemies)
        {
            for (int i = 0; i < numEnemies; i++)
            {
                Chara enemy = CreateEnemy(dangerLv);
                EClass._zone.AddCard(enemy, EClass.pc.pos.GetRandomPointInRadius(10, 30, requireLos: false));
                enemies.Add(enemy.uid);
            }
        }

        internal virtual Chara CreateEnemy(int dangerLv)
        {
            // クソクエになるのでリッチは出禁！！！！
            var spawnCharaSource = SpawnListChara.Get("all", (SourceChara.Row r) => r.race_row.IsUndead && r.race != "lich").Select(lv: dangerLv);
            int charaLv = (spawnCharaSource.LV + ((dangerLv >= 51) ? 50 : 0)) * Mathf.Max(1, (dangerLv - 1) / 50);

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

        internal virtual void SetConcertHallPiece()
        {
            GenBounds genBounds = GenBounds.Create(EClass._zone);
            genBounds.marginPartial = 1;
            genBounds.FuncCheckEmpty = (Cell cell) => cell.sourceFloor.id == 76;
            for (int i = 0; i < 25; i++)
            {
                genBounds.TryAddMapPiece(MapPiece.Type.Concert, 0f, "");
            }

            foreach (Thing thing in EClass._map.things)
            {
                thing.isNPCProperty = true;
            }
        }

    }
}
