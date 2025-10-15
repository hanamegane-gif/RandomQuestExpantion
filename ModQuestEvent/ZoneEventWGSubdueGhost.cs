using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventWGSubdueGhost : ZoneEventSubdueGhost
    {
        public override void OnVisit()
        {
            base.OnVisit();
            if (!EClass.game.isLoading)
            {
                EClass._zone.parent = MageGuildZone;
            }
        }
    }
}
