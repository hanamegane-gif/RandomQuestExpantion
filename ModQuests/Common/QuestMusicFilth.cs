using Newtonsoft.Json;
using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuestZoneInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomQuestExpantion.ModQuests.Common
{
    internal class QuestMusicFilth : QuestMusicMonster
    {
        public override string IdZone => "instance_music_filth";

        public override int DangerLv => partyLv + 5;

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventMusicMonsterFilth();
        }

        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceMusicFilth();
        }
    }
}
