using System;
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
            var centerPos = EClass._map.GetCenterPos();
            PartialMap.Apply("Special/farm_chest.mp", centerPos);
            var genBounds = GenBounds.Create(EClass._zone);
            genBounds.marginPartial = 1;
            genBounds.FuncCheckEmpty = (Cell cell) => cell.sourceFloor.id == 42;
            var crops = EClass.sources.objs.rows.Where((SourceObj.Row o) => o.tag.Contains("harvest")).ToList();

            Action<PartialMap, GenBounds> onCreate = delegate (PartialMap p, GenBounds b)
            {
                var list = b.ListEmptyPoint();
                var row = crops.RandomItemWeighted((SourceObj.Row o) => o.chance);
                int num = 1 + EClass.rnd(5 + base.quest.difficulty * 2);
                foreach (var item in list)
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

            foreach (var thing in EClass._map.things)
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
