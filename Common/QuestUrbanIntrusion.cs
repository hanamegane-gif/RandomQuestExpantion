using RandomQuestExpantion.ModQuestZoneInstance;
using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.Config;
using static RandomQuestExpantion.General.General;
using UnityEngine;
using System;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestUrbanIntrusion : QuestSubdue
    {
        public override string IdZone => "instance_" + ((this.chara.currentZone.IsTown || IsGuild(this.chara.currentZone)) ? this.chara.currentZone.id : "tinkerCamp");

        public override int BaseMoney => (int)Math.Min((long)source.money + (long)EClass.curve(DangerLv, 500, 2000, 90) * 2L, Int32.MaxValue / 200);

        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceUrbanIntrusion();
        }

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventUrbanIntrusion();
        }

        public override int GetRewardPlat(int money)
        {
            int bonusPlat = (ModConfig.RewardPlatRate > 0) ? EClass.curve(DangerLv / 25, 10, 20, 80) * ModConfig.RewardPlatRate / 100 : EClass.rndHalf((int)Mathf.Sqrt(money / 200));
            return 1 + EClass.rnd(2) + bonusPlat;
        }
    }
}
