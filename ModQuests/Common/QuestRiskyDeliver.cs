using Newtonsoft.Json;
using RandomQuestExpantion.ModZonePreenter;
using System;
using System.Linq;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestRiskyDeliver : QuestDeliver, IOverrideWildernessEncounter
    {
        [JsonProperty]
        public int TravelEncounterdCount = 0;

        public override bool ForbidTeleport => false;

        public int KOROSHITEDEMOUBAITORUChance => 10;

        [JsonProperty]
        public Thing Distribution = null;

        public bool CanDeliver => Distribution != null;

        // 初回:90% 2回目: 40% 3回目: 15%
        public int EncounterChance => (100 / (int)(Math.Pow(2, TravelEncounterdCount))) - 10;

        public override void OnStart()
        {
            var thing = GenerateDistribution();
            SetDistribution(thing);
            Msg.Say("get_quest_item");
            EClass.pc.Pick(thing);
        }

        public override void OnComplete()
        {
            base.OnComplete();
            if (EClass.rnd(100) >= KOROSHITEDEMOUBAITORUChance)
            {
                Distribution.Destroy();
            }
        }

        public override bool IsDestThing(Thing t)
        {
            if (Distribution == null)
            {
                return false;
            }

            if (t.parentCard != null && !t.parentCard.trait.CanUseContent)
            {
                return false;
            }

            if (t.uid != Distribution.uid)
            {
                return false;
            }

            if (!t.c_isImportant && (!t.IsContainer || t.things.Count == 0) && !t.isEquipped)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void OnWildernessEncounted(Zone newZone)
        {
            TravelEncounterdCount++;
            newZone.events.AddPreEnter(new ZonePreEnterDeliverBrigand(Distribution));
        }

        public virtual bool ShouldOverrideEncounter()
        {
            return ListDestThing(onlyFirst: true).Any() && (EClass.rnd(100) < EncounterChance);
        }

        public virtual int GetOverlappingPriority()
        {
            return 4 - TravelEncounterdCount;
        }

        public virtual Thing GenerateDistribution()
        {
            var thing = ThingGen.Create(idThing);
            thing.Identify(show: false, IDTSource.SuperiorIdentify);

            return thing;
        }

        public void SetDistribution(in Thing _distribution)
        {
            Distribution = _distribution;
        }
    }
}
