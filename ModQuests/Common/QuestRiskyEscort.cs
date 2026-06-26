using Newtonsoft.Json;
using RandomQuestExpantion.Config;
using RandomQuestExpantion.ModQuests.QuestAttribute;
using RandomQuestExpantion.ModZonePreenter;
using System;
using System.Collections.Generic;
using UnityEngine;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.Common
{
    public class QuestRiskyEscort : QuestEscort, IOverrideWildernessEncounter, IExtendDeadline
    {
        [JsonProperty]
        public string EscortTargetName = "";

        [JsonProperty]
        public int TravelEncounterdCount = 0;

        public override bool ForbidTeleport => false;

        public override bool FameContent => true;

        internal virtual HashSet<string> SpawnCandidateList => new HashSet<string>
        {
            "rich",
        };

        public override void OnStart()
        {
            var chara = SpawnEscortTarget();
            SetEscortTarget(chara);
            chara.Talk("parasite", null, null, forceSync: true);
            OnStartExtendDeadline();
        }

        // 初回:90% 2回目: 40% 3回目: 15%
        public int EncounterChance => (100 / (int)(Math.Pow(2, TravelEncounterdCount))) - 10;

        public virtual int DaysExtraDeadline => 2 + (7 - Mathf.Clamp(this.difficulty, 1, 7)) / 2;

        public override int BaseMoney => (int)Math.Min((long)source.money + (long)EClass.curve(DangerLv, 500, 2000, 90) * 2L, Int32.MaxValue / 200);

        public virtual void OnWildernessEncounted(Zone newZone)
        {
            TravelEncounterdCount++;
            newZone.events.AddPreEnter(new ZonePreEnterEscortAssassin(this));
        }

        public virtual Chara SpawnEscortTarget()
        {
            var chara = CharaGen.Create(SpawnCandidateList.RandomItem(), lv: this.dangerLv);
            int charaLv = EClass.curve(this.dangerLv / 4 + 10, 120, 200, 90);
            chara.SetLv(charaLv);

            EClass._zone.AddCard(chara, EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false));
            chara.MakeMinion(EClass.pc);

            return chara;
        }

        public virtual bool ShouldOverrideEncounter()
        {
            return EClass.rnd(100) < EncounterChance;
        }

        public virtual int GetOverlappingPriority()
        {
            return 4 - TravelEncounterdCount;
        }

        public override int GetRewardPlat(int money)
        {
            int bonusPlat = (ModConfig.RewardPlatRate > 0) ? EClass.curve(DangerLv / 25, 10, 20, 80) * ModConfig.RewardPlatRate / 100 : EClass.rndHalf((int)Mathf.Sqrt(money / 200));
            return 1 + EClass.rnd(2) + bonusPlat;
        }

        public void SetEscortTarget(in Chara spawned)
        {
            uidChara = spawned.uid;
            EscortTargetName = spawned.GetName(NameStyle.Full);
        }

        public Quest OnStartExtendDeadline()
        {
            deadline += DaysExtraDeadline * Date.DayToken;
            return this;
        }

        public string GetAltTextDeadline()
        {
            return AltTextDeadline((Hours >= 0) ? Hours : 0, DaysExtraDeadline);
        }
    }
}
