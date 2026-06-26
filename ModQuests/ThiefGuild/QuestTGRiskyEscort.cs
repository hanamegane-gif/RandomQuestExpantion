using RandomQuestExpantion.ModQuests.Common;
using RandomQuestExpantion.ModZonePreenter;
using System.Collections.Generic;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.ThiefGuild
{
    public class QuestTGRiskyEscort : QuestRiskyEscort
    {
        internal override HashSet<string> SpawnCandidateList => new HashSet<string>
        {
            "rogue",
            "guild_thief",
            "hitman",
            "thief",
            "ratkin",
            "prisoner",
        };

        public override string RewardSuffix => "_byakko_mod_guild";

        public override void OnWildernessEncounted(Zone newZone)
        {
            TravelEncounterdCount++;
            newZone.events.AddPreEnter(new ZonePreEnterTGEscortAssassin(this));
        }

        public override int GetRewardPlat(int money)
        {
            return 1 + EClass.rnd(2);
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            int guilpoNum = 2;
            var guilpo = ThingGen.Create("MOD_byakko_RQX_guilpo_thief").SetNum(guilpoNum);
            DropReward(guilpo);
            MerchantGuildZone.ModInfluence(1);
        }
    }
}
