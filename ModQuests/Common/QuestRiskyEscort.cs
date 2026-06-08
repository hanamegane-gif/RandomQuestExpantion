using Newtonsoft.Json;
using RandomQuestExpantion.ModZonePreenter;
using System;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestRiskyEscort : QuestEscort, IOverrideWildernessEncounter
    {
        [JsonProperty]
        public string TargetName = "";

        [JsonProperty]
        public int TravelEncounterdCount = 0;

        public override bool ForbidTeleport => false;

        public override void OnStart()
        {
            var chara = SpawnEscortTarget();
            SetEscortTarget(chara);
            chara.Talk("parasite", null, null, forceSync: true);
        }

        // 初回:90% 2回目: 40% 3回目: 15%
        public int EncounterChance => (100 / (int)(Math.Pow(2, TravelEncounterdCount))) - 10;

        public virtual void OnWildernessEncounted(Zone newZone)
        {
            TravelEncounterdCount++;
            newZone.events.AddPreEnter(new ZonePreEnterEscortAssassin(TargetName));
        }

        public virtual Chara SpawnEscortTarget()
        {
            var chara = CharaGen.CreateFromFilter("c_neutral", 10);
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

        public void SetEscortTarget(in Chara spawned)
        {
            uidChara = spawned.uid;
            TargetName = spawned.GetName(NameStyle.Full);
        }
    }
}
