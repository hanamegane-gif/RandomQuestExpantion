using Newtonsoft.Json;
using UnityEngine;

namespace RandomQuestExpantion.ModQuestTask
{
    class TaskDungeonAttack : QuestTask
    {
        [JsonProperty]
        public bool hasNefiaBossKilled = false;

        public override bool IsComplete()
        {
            return hasNefiaBossKilled;
        }

        public virtual void OnNefiaBeaten(Chara boss)
        {
            hasNefiaBossKilled = true;
            owner.bonusMoney += CalcBonusMoney(boss);
        }

        internal virtual int CalcBonusMoney(in Chara boss)
        {
            int baseMoney = Mathf.Clamp((3 + boss.LV) * 10, 40, 20000000);

            // curveは使うがバニラ依頼の強敵ボーナスはあまりにもしょっぱすぎるのでrate高め
            return EClass.curve(baseMoney, 500, 2000, 90);
        }

        public override string GetTextProgress()
        {
            return "byakko_mod_progress_dungeon_attack".lang();
        }

        public override void OnGetDetail(ref string detail, bool onJournal)
        {
            return;
        }
    }
}
