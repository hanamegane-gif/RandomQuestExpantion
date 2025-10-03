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

        public override void OnKillChara(Chara c)
        {
            if (c.IsPCFaction)
            {
                return;
            }

            if (IsNefiaBoss(c))
            {
                hasNefiaBossKilled = true;
                owner.bonusMoney += CalcBonusMoney(c);
            }
        }
        public override string GetTextProgress()
        {
            return "byakko_mod_progress_dungeon_attack".lang();
        }

        public override void OnGetDetail(ref string detail, bool onJournal)
        {
            return;
        }

        private int CalcBonusMoney(in Chara boss)
        {
            int baseMoney = Mathf.Clamp((3 + boss.LV) * 10, 40, 100000000);

            // curveは使うがバニラ依頼の強敵ボーナスはあまりにもしょっぱすぎるのでrate高め
            return EClass.curve(baseMoney, 500, 2000, 90);
        }

        private bool IsNefiaBoss(Chara killedChara)
        {
            // ボス討伐時はzone.Bossをnullにした後にOnKillCharaが呼ばれるため、EClass._zone.Boss == killedCharaで楽に判定できない
            // まともにネフィアの主を判定する方法がないので力業

            // 「ネフィアの主＝最下層にいるボス」として、最下層かどうかはShouldMakeExitで判定する
            if (!EClass._zone.IsNefia || EClass._zone.ShouldMakeExit)
            {
                return false;
            }

            // 争いの祠で出てくるのもボス扱いなので引っかからないようにする
            if (EClass._zone.Boss != null)
            {
                return false;
            }

            return killedChara.c_bossType == BossType.Boss;
        }
    }
}
