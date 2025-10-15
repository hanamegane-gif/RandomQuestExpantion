using UnityEngine;

namespace RandomQuestExpantion.ModQuestTask
{
    // 討伐Task自体は本体側にあるが読みづらい上に使いにくいので自前で用意する
    class TaskHunt : QuestTaskHunt
    {
        public override string RefDrama1 => numHunted.ToString();

        public override string RefDrama2 => numRequired.ToString();

        public override bool IsComplete()
        {
            return numHunted >= numRequired;
        }

        public override void OnInit()
        {
            numRequired = 10 + owner.difficulty * 3 + EClass.rnd(5);
        }

        public override void OnKillChara(Chara c)
        {
            if (c.IsPCFaction)
            {
                return;
            }

            if (!IsComplete() && c.OriginalHostility == Hostility.Enemy)
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

    }
}
