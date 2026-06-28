using RandomQuestExpantion.General;
using System;
using System.Collections.Generic;

namespace RandomQuestExpantion.ModQuestEvent
{
    internal class ZoneEventOrganizingBook : ZoneEventHarvest
    {
        public override string TextWidgetDate => "byakko_mod_status_book_recover".lang((TimeLimit - minElapsed <= 30) ? "end_soon".lang() : "", (questHarvest.weightDelivered / 20).ToString(), (questHarvest.destWeight / 20).ToString()) + "byakko_mod_progress_timer".lang((TimeLimit - minElapsed).ToString());

        internal virtual HashSet<string> SpawnCandidateList => new HashSet<string>
        {
            "fairy",
            "child_fairy", // こいつは海外兄貴達に怒られたら消す
            "imp",
            "goblin_wizard",
            "yeek_master",
            "bat",
            "spider_spotted",
            "grudge",
            "ghost",
        };

        public override void OnVisit()
        {
            if (EClass.game.isLoading)
            {
                return;
            }

            EClass._zone.SetBGM(82);
            EClass._zone.AddCard(ThingGen.Create("container_shipping_farm"), new Point(50, 49)).Install().isNPCProperty = true;
            EClass._zone.AddCard(ThingGen.Create("376"), new Point(49, 49)).Install().isNPCProperty = true;

            var genBounds = GenBounds.Create(EClass._zone);
            genBounds.marginPartial = 1;

            // インスタンスマップにはカーペット(id65)を敷き詰めた
            genBounds.FuncCheckEmpty = (Cell cell) => cell.sourceFloor.id == 65;

            Action<PartialMap, GenBounds> onCreate = delegate (PartialMap p, GenBounds b)
            {
                var list = b.ListEmptyPoint();
                for (int i = 0; i < 1 + EClass.rnd(2); i++)
                {
                    if (list.Count == 0)
                    {
                        break;
                    }
                    var spawnPoint = list.RandomItem();
                    var extraChara = CreateExtraChara();
                    EClass._zone.AddCard(extraChara, spawnPoint);

                    var book = (EClass.rnd(5) == 0) ? CreateSpellBook() : CreateAncientBook();
                    EClass._zone.AddCard(book, spawnPoint.GetNearestPoint(minRadius: 1));
                    list.Remove(spawnPoint);
                }
            };

            for (int i = 0; i < 50; i++)
            {
                string pieceType = EClass.rnd(2) == 0 ? "study" : "study_room";
                ModMapPiece.TryAddMapPiece(genBounds, pieceType, onCreate);
            }

            foreach (var thing in EClass._map.things)
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

            var list = new List<Thing>();
            foreach (var member in EClass.pc.party.members)
            {
                member.things.Foreach(delegate (Thing t)
                {
                    if (t.GetBool(115) || EClass.rnd(2) != 0)
                    {
                        return;
                    }

                    if (t.id == "book_ancient" || t.id == "spellbook")
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
            foreach (var item in list)
            {
                item.Destroy();
            }
        }

        // 賑やかしさんを作る
        internal virtual Chara CreateExtraChara()
        {
            var createdChara = CharaGen.Create(SpawnCandidateList.RandomItem());
            createdChara.c_originalHostility = Hostility.Enemy;
            createdChara.hostility = Hostility.Enemy;

            return createdChara;
        }

        internal virtual Thing CreateSpellBook()
        {
            var book = ThingGen.Create("372", lv: 25);
            book.SetBool(115, true);
            book.Identify(show: false);

            return book;
        }

        internal virtual Thing CreateAncientBook()
        {
            var book = ThingGen.Create("book_ancient", lv: 20);
            book.SetBool(115, true);
            book.isOn = (EClass.rnd(4) != 0);
            book.Identify(show: false);

            return book;
        }
    }
}
