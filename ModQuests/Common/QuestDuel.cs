using RandomQuestExpantion;
using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuestZoneInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RandomQuestExpantion.ModQuests.Common
{
    public class QuestDuel : QuestSubdue
    {
        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventDuel();
        }

        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceDuel();
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2) + EClass.curve(DangerLv / 5, 20, 20, 85);
        }
    }
}
