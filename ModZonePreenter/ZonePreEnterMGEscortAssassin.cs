using RandomQuestExpantion.ModQuests.Common;
using System.Collections.Generic;

namespace RandomQuestExpantion.ModZonePreenter
{
    public class ZonePreEnterMGEscortAssassin : ZonePreEnterEscortAssassin
    {
        public ZonePreEnterMGEscortAssassin(QuestRiskyEscort q) : base(q)
        {
        }

        internal override HashSet<string> DramaStartStepList => new HashSet<string>
        {
            "assassin_MG_nonego_1_main",
            "assassin_MG_nego_1_main",
        };
    }
}
