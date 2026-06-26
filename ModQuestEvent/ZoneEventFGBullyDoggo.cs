using RandomQuestExpantion.ModQuests.Common;
using System.Collections.Generic;

namespace RandomQuestExpantion.ModQuestEvent
{
    internal class ZoneEventFGBullyDoggo : ZoneEventBullyDoggo
    {
        internal override HashSet<string> DramaStartStepList => new HashSet<string>
        {
            "doggohint_FG_1",
        };

        public ZoneEventFGBullyDoggo(QuestBullyDoggo ownerQuest) : base(ownerQuest)
        {
        }
    }
}
