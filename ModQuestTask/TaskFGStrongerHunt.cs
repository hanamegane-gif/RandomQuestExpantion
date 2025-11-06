namespace RandomQuestExpantion.ModQuestTask
{
    class TaskFGStrongerHunt : TaskFGHunt
    {
        public override string RefDrama3 => NefiaDangerLvRequirement.ToString();
        public int NefiaDangerLvRequirement => (this.owner.DangerLv * 3) / 4;

        public override void OnKillChara(Chara c)
        {
            if (c.IsPCFaction || c.OriginalHostility != Hostility.Enemy)
            {
                return;
            }

            if (EClass._zone.IsNefia && EClass._zone.DangerLv >= NefiaDangerLvRequirement)
            {
                numHunted++;
            }
        }

        public override string GetTextProgress()
        {
            return "byakko_mod_progress_stronger_hunt".lang(RefDrama1, RefDrama2, RefDrama3);
        }
    }
}
