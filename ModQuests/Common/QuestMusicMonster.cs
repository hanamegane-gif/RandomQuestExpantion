using Newtonsoft.Json;
using RandomQuestExpantion.Config;
using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuestZoneInstance;
using UnityEngine;

namespace RandomQuestExpantion.ModQuests.Common
{
    internal class QuestMusicMonster : QuestMusic
    {
        public override string IdZone => "instance_music_noble";

        // 難易度タイプが音楽だと★7に偏りすぎる
        public override DifficultyType difficultyType => DifficultyType.Default;

        public override int BaseMoney => source.money + EClass.curve(DangerLv, 20, 15) * 10;

        public override string RewardSuffix => "Music";

        public override bool FameContent => true;

        public override int DangerLv => partyLv + 10;

        public override int KarmaOnFail => 0;

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventMusicMonster();
        }

        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceMusic();
        }

        public override string GetTextProgress()
        {
            var @event = EClass._zone.events.GetEvent<ZoneEventMusicMonster>();
            return "byakko_mod_progress_music_monster".lang(@event.GetVoltageString);
        }

        public override int GetRewardPlat(int money)
        {
            int bonusPlat = (ModConfig.RewardPlatRate > 0) ? EClass.curve(money / 400, 3, 10, 65) * ModConfig.RewardPlatRate / 100 : EClass.rndHalf((int)Mathf.Sqrt(money / 200));

            return (3 + difficulty + EClass.rnd(2)) + bonusPlat;
        }

        public override void OnInit()
        {
            partyLv = EClass.pc.elements.ValueWithoutLink(SKILL.music);

            destScore = difficulty * 150;
            destScore += EClass.rnd(destScore / 5);
        }
    }
}
