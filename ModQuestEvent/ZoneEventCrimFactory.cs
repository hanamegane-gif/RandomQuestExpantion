using RandomQuestExpantion.General;
using System;
using System.Collections.Generic;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventCrimFactory : ZoneEventHarvest
    {
        public override string TextWidgetDate => "byakko_mod_status_crim_produce".lang((TimeLimit - minElapsed <= 30) ? "end_soon".lang() : "", Lang._weight(questHarvest.weightDelivered), Lang._weight(questHarvest.destWeight)) + "byakko_mod_progress_timer".lang((TimeLimit - minElapsed).ToString());

        // ならず者はすれ違いのスリが割とストレス要因になるため出禁
        internal virtual HashSet<string> SpawnCandidateList { get; } = new HashSet<string>
        {
            "punk",
            "guild_thief",
            "gangster",
            "prisoner",
            "begger",
            "soldier_fallen",
        };

        public override void OnVisit()
        {
            if (EClass.game.isLoading)
            {
                return;
            }

            EClass._zone.SetBGM(79);
            Point centerPos = EClass._map.GetCenterPos();
            EClass._zone.AddCard(ThingGen.Create("container_shipping_farm"), centerPos).Install().isNPCProperty = true;
            EClass._zone.AddCard(ThingGen.Create("376"), centerPos.GetRandomNeighbor()).Install().isNPCProperty = true;

            GenBounds genBounds = GenBounds.Create(EClass._zone);
            genBounds.marginPartial = 1;

            // インスタンスマップにはチョコレートの床を敷き詰めた
            genBounds.FuncCheckEmpty = (Cell cell) => cell.sourceFloor.id == 123;

            Action<PartialMap, GenBounds> onCreate = delegate (PartialMap p, GenBounds b)
            {
                List<Point> list = b.ListEmptyPoint();
                for (int i = 0; i < EClass.rndHalf(5); i++)
                {
                    if (list.Count == 0)
                    {
                        break;
                    }
                    Point spawnPoint = list.RandomItem();
                    Chara extraChara = CreateExtraChara();
                    EClass._zone.AddCard(extraChara, spawnPoint);
                    list.Remove(spawnPoint);
                }
            };

            for (int i = 0; i < 50; i++)
            {
                string pieceType = EClass.rnd(4) == 0 ? "crim_factory" : "crim_crop";
                ModMapPiece.TryAddMapPiece(genBounds, pieceType, onCreate);
            }

            foreach (Thing thing in EClass._map.things)
            {
                thing.isNPCProperty = true;
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
                    if ((t.id == "crim" || t.id == "drug_crim") && EClass.rnd(2) != 0)
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

        // 賑やかしさんを作る
        internal virtual Chara CreateExtraChara()
        {
            var spawnCharaSource = SpawnListChara.Get("all", (SourceChara.Row r) => SpawnCandidateList.Contains(r.id)).Select(lv: 50);

            Chara createdChara = CharaGen.Create(spawnCharaSource.id);
            createdChara.c_originalHostility = Hostility.Neutral;
            createdChara.hostility = Hostility.Neutral;
            createdChara.AddCondition<ConHallucination>(10000, force: true);
            createdChara.AddCondition<ConDrug>(10000, force: true);

            // 赤本曰くアッパー系らしい
            int conditionRoll = EClass.rnd(100);
            if (conditionRoll < 26)
            {
                createdChara.AddCondition<ConSmoking>(10000, force: true);
            }
            else if (conditionRoll < 52)
            {
                createdChara.AddCondition<ConAwakening>(10000, force: true);
            }
            else if (conditionRoll < 78)
            {
                createdChara.AddCondition<ConEuphoric>(10000, force: true);
            }
            else if (conditionRoll < 89)
            {
                createdChara.AddCondition<ConConfuse>(10000, force: true);
            }
            else
            {
                createdChara.AddCondition<ConParalyze>(10000, force: true);
            }

            return createdChara;
        }
    }
}
