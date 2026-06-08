using Newtonsoft.Json;
using UnityEngine;

namespace RandomQuestExpantion.ModQuestTask
{
    class TaskBullyDoggo : QuestTaskHunt
    {
        [JsonProperty]
        public int UIDDoggo = -1;

        public override string RefDrama1 => numHunted.ToString();

        public override string RefDrama2 => numRequired.ToString();

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
                owner.bonusMoney += CalcBonusMoney(c);
            }
        }

        internal virtual int CalcBonusMoney(in Chara killedChara)
        {
            int baseMoney = Mathf.Clamp(3 + killedChara.LV, 4, 20000000);
            return EClass.curve(baseMoney, 50, 10);
        }

        public override string GetTextProgress()
        {
            return "progressHunt".lang(RefDrama1, RefDrama2);
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
