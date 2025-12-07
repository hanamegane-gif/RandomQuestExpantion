using RandomQuestExpantion.General;
using System;
using System.Collections.Generic;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventHerbHarvest : ZoneEventHarvest
    {
        public override string TextWidgetDate => "eventHarvest".lang((TimeLimit - minElapsed <= 30) ? "end_soon".lang() : "", Lang._weight(questHarvest.weightDelivered), Lang._weight(questHarvest.destWeight)) + "byakko_mod_progress_timer".lang((TimeLimit - minElapsed).ToString());

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

            Action<PartialMap, GenBounds> onCreate = delegate (PartialMap p, GenBounds b){};

            for (int i = 0; i < 50; i++)
            {
                ModMapPiece.TryAddMapPiece(genBounds, "herb", onCreate);
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

        public override void OnLeaveZone()
        {
            if (EClass._zone.instance.status == ZoneInstance.Status.Running)
            {
                EClass._zone.instance.status = OnReachTimeLimit();
            }

            List<Thing> list = new List<Thing>();
            foreach (Chara member in EClass.pc.party.members)
            {
                member.things.Foreach(delegate (Thing t)
                {
                    // バレづらい
                    if ((t.id == "herb_red" || t.id == "herb_blue" || t.id == "herb_purple" || t.id == "herb_green") && EClass.rnd(4) != 0)
                    {
                        list.Add(t);
                    }
                });
            }

            if (list.Count <= 0)
            {
                return;
            }

            Msg.Say("harvest_confiscate", list.Count.ToString() ?? "");
            foreach (Thing item in list)
            {
                item.Destroy();
            }
        }
    }
}
