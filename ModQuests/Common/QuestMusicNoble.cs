using Newtonsoft.Json;
using RandomQuestExpantion.Config;
using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuestZoneInstance;
using UnityEngine;

namespace RandomQuestExpantion.ModQuests.Common
{
    internal class QuestMusicNoble : QuestMusicMonster
    {
        public override string IdZone => "instance_music_noble";

        public override int DangerLv => partyLv + 10;

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventMusicMonsterNoble();
        }

        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceMusicNoble();
        }
    }
}
