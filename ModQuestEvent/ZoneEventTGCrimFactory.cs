using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventTGCrimFactory : ZoneEventCrimFactory
    {
        public override void OnVisit()
        {
            if (EClass.game.isLoading)
            {
                return;
            }

            base.OnVisit();
            EClass._zone.parent = ThiefGuildZone;
        }
    }
}
