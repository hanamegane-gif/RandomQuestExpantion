using Newtonsoft.Json;
using RandomQuestExpantion.DayBreak;
using UnityEngine;

namespace RandomQuestExpantion.ModQuestTask
{
    class TaskBullyDoggo : QuestTaskHunt
    {
        [JsonProperty]
        public int UIDDoggo = -1;

        public override void OnInit()
        {
            numRequired = 1;
        }

        public override void OnKillChara(Chara c)
        {
            if (c.IsPCFaction)
            {
                return;
            }

            if (!IsComplete() && IsDoggo(c))
            {
                numHunted++;
                owner.SetBonusMoney(owner.GetBonusMoney() + CalcBonusMoney(c));
            }
        }

        internal virtual int CalcBonusMoney(in Chara killedChara)
        {
            int baseMoney = Mathf.Clamp(3 + killedChara.LV, 4, 20000000);
            return EClass.curve(baseMoney, 50, 10);
        }

        public override string GetTextProgress()
        {
            return "byakko_mod_progress_doggo".lang();
        }

        public override void OnGetDetail(ref string detail, bool onJournal)
        {
            return;
        }

        internal void SetUIDDoggo(int uid)
        {
            UIDDoggo = uid;
        }

        internal bool IsDoggo(in Chara c)
        {
            return c.uid == UIDDoggo;
        }
    }
}
