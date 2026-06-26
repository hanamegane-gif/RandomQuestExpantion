using RandomQuestExpantion.ModQuests.Common;
using System.Collections.Generic;

namespace RandomQuestExpantion.ModZonePreenter
{
    class ZonePreEnterTGEscortAssassin : ZonePreEnterEscortAssassin
    {
        public ZonePreEnterTGEscortAssassin(QuestRiskyEscort q) : base(q)
        {
        }

        internal override HashSet<string> DramaStartStepList => new HashSet<string>
        {
            "assassin_TG_nonego_1_main",
            "assassin_TG_nego_1_main",
        };
    }
}
