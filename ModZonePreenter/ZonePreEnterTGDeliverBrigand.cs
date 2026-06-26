using RandomQuestExpantion.ModQuests.Common;
using System.Collections.Generic;

namespace RandomQuestExpantion.ModZonePreenter
{
    class ZonePreEnterTGDeliverBrigand : ZonePreEnterDeliverBrigand
    {
        public ZonePreEnterTGDeliverBrigand(QuestRiskyDeliver q) : base(q)
        {
        }

        internal override HashSet<string> DramaStartStepList => new HashSet<string>
        {
            "brigand_TG_nonego_1_main",
            "brigand_TG_nego_1_main",
        };
    }
}
