using System;
using System.Collections.Generic;
using System.Linq;
using RandomQuestExpantion.General;
using UnityEngine;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventFish : ZoneEventHarvest
    {
        public override void OnVisit()
        {
            if (EClass.game.isLoading)
            {
                return;
            }

            EClass._zone.SetBGM(17);
            Point centerPos = EClass._map.GetCenterPos();
            PartialMap.Apply("Special/farm_chest.mp", centerPos);
            GenBounds genBounds = GenBounds.Create(EClass._zone);
            genBounds.marginPartial = 1;
            genBounds.FuncCheckEmpty = (Cell cell) => cell.sourceFloor.id == 42;
            List<SourceObj.Row> crops = EClass.sources.objs.rows.Where((SourceObj.Row o) => o.tag.Contains("harvest")).ToList();

            Action<PartialMap, GenBounds> onCreate = delegate (PartialMap p, GenBounds b)
            {
                List<Point> list = b.ListEmptyPoint();
                SourceObj.Row row = crops.RandomItemWeighted((SourceObj.Row o) => o.chance);
                int num = 1 + EClass.rnd(5 + base.quest.difficulty * 2);
                foreach (Point item in list)
                {
                    if (item.sourceFloor.id == 4 && EClass.rnd(4) != 0)
                    {
                        item.SetObj(row.id);
                        int num2 = item.growth.HarvestStage - EClass.rnd(4);
                        item.growth.SetStage(num2);
                        if (num2 == item.growth.HarvestStage)
                        {
                            EClass._map.AddPlant(item, null).size = Mathf.Clamp(num + EClass.rnd(2) - EClass.rnd(2), 0, 9) + 1;
                        }

                        item.cell.isClearSnow = true;
                    }
                }
            };

            for (int i = 0; i < 50; i++)
            {
                ModMapPiece.TryAddMapPiece(genBounds, "fish", onCreate);
            }

            foreach (Thing thing in EClass._map.things)
            {
                thing.isNPCProperty = true;
            }

            for (int j = 0; j < 12; j++)
            {
                EClass._zone.SpawnMob();
            }

            for (int k = 0; k < 30; k++)
            {
                EClass._zone.SpawnMob(null, SpawnSetting.HomeWild(1));
            }
        }
    }
}
