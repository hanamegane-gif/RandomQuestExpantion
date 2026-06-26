using RandomQuestExpantion.General;
using RandomQuestExpantion.ModQuests.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RandomQuestExpantion.ModZonePreenter
{
    public class ZonePreEnterEscortAssassin : ZonePreEnterEvent
    {
        private QuestRiskyEscort _Quest;

        internal virtual HashSet<string> SpawnCandidateList => new HashSet<string>
        {
            "merc",
            "merc_archer",
            "merc_mage",
            "merc_warrior",
        };

        internal virtual HashSet<string> DramaStartStepList => new HashSet<string>
        {
            //"assassin_common_nonego_1_main",
            //"assassin_common_nego_1_main",
        };

        public ZonePreEnterEscortAssassin(QuestRiskyEscort q)
        {
            _Quest = q;
        }

        public override void Execute()
        {
            var spawnCenterPoint = EClass.pc.pos.GetRandomPointInRadius(2, 5, requireLos: false, allowChara: false);
            var leader = SpawnLeader(spawnCenterPoint);

            if (leader == null)
            {
                return;
            }

            var offerMoney = GenerateOfferMoney(leader);
            var omakeGoods = GenerateOmakeGoods(leader);

            var callbackAction = new Action(() =>
            {
                if (DramaWrapper.GetDramaResult<bool>("RQX_negoaccept"))
                {
                    leader.hostility = leader.c_originalHostility = Hostility.Neutral;

                    EClass.pc.PickOrDrop(EClass.pc.pos, offerMoney);
                    if (DramaWrapper.GetDramaResult<bool>("RQX_negoraise"))
                    {
                        EClass.pc.PickOrDrop(EClass.pc.pos, omakeGoods);
                    }

                    _Quest.Fail();
                }
                else
                {
                    SpawnMobs(spawnCenterPoint, 11 + EClass.rnd(5));
                }
                DramaWrapper.Release();
            });

            DramaWrapper.Lock();
            DramaWrapper.SetArgumentBoolean("RQX_canraise", CalcCanRaise(leader));
            DramaWrapper.SetArgumentStrings(_Quest.EscortTargetName, Lang._currency(offerMoney.Num, showUnit: true), omakeGoods.GetName(NameStyle.Full));
            DramaWrapper.SetCallbackAction(callbackAction);
            DramaWrapper.PlayDrama(leader, DramaStartStepList.RandomItem());
        }

        internal virtual Thing GenerateOfferMoney(Chara leader)
        {
            int lvBonus = EClass.curve(leader.LV, 2000, 2000, 90) * 5 / 2;
            int num = 1000 + lvBonus / 2 + EClass.rnd(lvBonus / 2 + 1000);

            return ThingGen.Create("money").SetNum(num);
        }

        internal virtual Thing GenerateOmakeGoods(Chara leader)
        {
            int filterRoll = EClass.rnd(100);
            int chanceSum = 0;
            if (filterRoll < (chanceSum += 20))
            {
                return ThingGen.Create("ticket_fortune").SetNum(6 + EClass.rnd(5));
            }
            else if (filterRoll < (chanceSum += 20))
            {
                return ThingGen.Create("medal").SetNum(4 + EClass.rnd(4));
            }
            else if (filterRoll < (chanceSum += 20))
            {
                return ThingGen.Create("ecopo").SetNum(EClass.rndHalf((int)Mathf.Sqrt(leader.LV / 4) + 5));
            }
            else if (filterRoll < (chanceSum += 15))
            {
                return ThingGen.Create("map_treasure", lv: leader.LV * 2);
            }
            else if (filterRoll < (chanceSum += 15))
            {
                var thing = ThingGen.Create("book_skill");
                thing.SetNum(3);
                return thing;
            }
            else if (filterRoll < (chanceSum += 5))
            {
                return ThingGen.Createジュアさまの薄い本();
            }
            else if (filterRoll < (chanceSum += 4))
            {
                return ThingGen.Create("goodness");
            }
            else
            {
                var thing = ThingGen.Create("rod_wish");
                thing.SetCharge(0);
                return thing;
            }
        }

        private Chara SpawnLeader(Point spawnCenterPoint)
        {
            var spawnPoint = spawnCenterPoint.GetRandomPointInRadius(1, 4, requireLos: false, allowChara: false);
            var spawned = EClass._zone.SpawnMob(spawnPoint, SpawnSetting.Mob("adv", fixedLv: _Quest.dangerLv));
            spawned.SetLv(_Quest.dangerLv);
            spawned.hostility = spawned.c_originalHostility = Hostility.Enemy;

            return spawned;
        }

        private void SpawnMobs(Point spawnCenterPoint, int spawnNum)
        {
            for (int i = 0; i < spawnNum; i++)
            {
                var spawnPoint = spawnCenterPoint.GetRandomPointInRadius(1, 4, requireLos: false, allowChara: false);
                var spawned = EClass._zone.SpawnMob(spawnPoint, SpawnSetting.Mob(SpawnCandidateList.RandomItem(), fixedLv: _Quest.dangerLv * 4 / 5));
                spawned.hostility = spawned.c_originalHostility = Hostility.Enemy;
            }
        }

        private static bool CalcCanRaise(Chara leader)
        {
            int negotiationLevel = EClass.pc.CHA + EClass.pc.Evalue(SKILL.negotiation);
            int chance = 40 + Mathf.Clamp((10 * negotiationLevel / leader.CHA), 0, 55);
            return chance > EClass.rnd(100);
        }
    }
}
