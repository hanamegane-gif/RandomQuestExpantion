using Newtonsoft.Json;
using RandomQuestExpantion.ModQuestEvent;
using System.Linq;
using UnityEngine;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.Common
{
    internal class QuestRoadDefence : QuestDefenseGame
    {
        public override string IdZone => (IsMofuVillage) ? "instance_mofuroad" : "instance_road";

        public override string RefDrama1 => Lang._currency(rewardMoney, showUnit: true, 0);

        public override string RefDrama2 => EClass.sources.races.rows.FirstOrDefault(r => r.id == TargetRaceId)?.GetName() ?? "";

        public override string RewardSuffix => "_byakko_mod_road_defence";

        public override int FameOnComplete => (LastWaveBonus * 2 + difficulty * 3) * (100 + bonus) / 100;

        [JsonProperty]
        // questpickerで無視されている時用の種族、Lv1のプチがいる
        public string TargetRaceId = "slime";

        [JsonProperty]
        public bool IsMofuVillage = false;

        public override bool FameContent => true;

        public override ZoneEventQuest CreateEvent()
        {
            return new ZoneEventRoadDefence();
        }

        public override ZoneInstanceRandomQuest CreateInstance()
        {
            return new ZoneInstanceDefense();
        }

        public override void OnInit()
        {
            int fameLv = EClass.pc.FameLv;
            dangerLv = Mathf.Max(fameLv - (fameLv / 10 + 5), 5);
            for (int i = 0; i < 200; i++)
            {
                SourceRace.Row row = EClass.sources.races.rows.RandomItem();

                // 種族リッチに属する奴何もかもがクソすぎ
                if (row.id == "rich")
                { 
                    continue;
                }

                if (CanSetTarget(row.id, dangerLv))
                {
                    TargetRaceId = row.id;
                    break;
                }
            }

            IsMofuVillage = IsMofuVillage(EClass._zone);
        }

        public override void OnBeforeComplete()
        {
            bonusMoney += bonus;
        }

        public override string GetTextProgress()
        {
            var @event = EClass._zone.events.GetEvent<ZoneEventRoadDefence>();
            if (@event == null)
            {
                return "";
            }
            return "byakko_mod_progress_road_defence".lang(@event.kills.ToString() ?? "", (@event.WaveKillRequirements * @event.WaveRequirements).ToString() ?? "");
        }


        public static bool CanSetTarget(string idRace, int lv)
        {
            return EClass.sources.charas.rows.Where(
                r => r.quality == 0 && 
                r.race == idRace && 
                r.chance > 0 && 
                r.hostility == ""/*Enemy*/ && 
                r.LV <= lv && 
                !r.actCombat.Where(s => s.StartsWith("ActEscape")).Any()
            ).Any();
        }
    }
}
