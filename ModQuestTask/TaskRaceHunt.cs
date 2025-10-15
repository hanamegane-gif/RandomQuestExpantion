using System;
using System.Collections.Generic;
using UnityEngine;

namespace RandomQuestExpantion.ModQuestTask
{
    // 種族討伐Task自体は本体側にあるが読みづらい上に使いにくいので自前で用意する
    class TaskRaceHunt : QuestTaskHunt
    {
        public override string RefDrama1 => numHunted.ToString();

        public override string RefDrama2 => Race.GetName() ?? "";

        public override string RefDrama3 => numRequired.ToString();


        public override bool IsComplete()
        {
            return numHunted >= numRequired;
        }

        public override void OnInit()
        {
            for (int i = 0; i < 100; i++)
            {
                SourceRace.Row row = EClass.sources.races.rows.RandomItem();
                if (ListTargets(row.id).Count != 0)
                {
                    idRace = row.id;
                    break;
                }
            }

            numRequired = 3 + owner.difficulty * 2 + EClass.rnd(5);
        }

        public override void OnKillChara(Chara c)
        {
            if (c.IsPCFaction || c.race.id != idRace)
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
            return EClass.curve(baseMoney, 50, 10) * 2;
        }

        public override string GetTextProgress()
        {
            // 混乱するが1,3,2の順が正しい　ぜんぶQuestTaskHuntが悪い
            return "progressHuntRace".lang(RefDrama1, RefDrama3, RefDrama2);
        }

        public override void OnGetDetail(ref string detail, bool onJournal)
        {
            if (!onJournal)
            {
                return;
            }

            List<SourceChara.Row> list = ListTargets(idRace);
            int num = 0;
            detail = detail + Environment.NewLine + Environment.NewLine + "target_huntRace".lang() + Environment.NewLine;
            foreach (SourceChara.Row item in list)
            {
                detail = detail + item.GetName().ToTitleCase(wholeText: true) + " (" + EClass.sources.races.map[idRace].GetName() + ")";
                num++;
                if (num > 5)
                {
                    break;
                }

                detail += Environment.NewLine;
            }
        }
    }
}
