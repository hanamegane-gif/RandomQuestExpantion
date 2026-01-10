using RandomQuestExpantion.Config;
using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuestZoneInstance;
using static RandomQuestExpantion.General.General;
using System;
using UnityEngine;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestUrbanDefence : QuestSubdue
    {
        public override string IdZone => "instance_" + ((this.chara.currentZone.IsTown || IsGuild(this.chara.currentZone)) ? this.chara.currentZone.id : "tinkerCamp");

        public override int BaseMoney => (int)Math.Min((long)source.money + (long)EClass.curve(DangerLv, 500, 2000, 90) * 3L, Int32.MaxValue / 200);

        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceUrbanDefence();
        }

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventUrbanDefence();
        }

        public override int GetRewardPlat(int money)
        {
            int bonusPlat = (ModConfig.RewardPlatRate > 0) ? EClass.curve(DangerLv / 25, 10, 20, 80) * ModConfig.RewardPlatRate / 100 : EClass.rndHalf((int)Mathf.Sqrt(money / 200));
            return 1 + EClass.rnd(2) + bonusPlat;
        }

        public override string GetTextProgress()
        {
            if (isComplete)
            {
                return "";
            }

            return "byakko_mod_progress_urban_defence".lang();
        }
    }
}
