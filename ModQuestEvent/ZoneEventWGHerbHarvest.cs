using static RandomQuestExpantion.General.General;
namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventWGHerbHarvest : ZoneEventHerbHarvest
    {
        public override void OnVisit()
        {
            if (EClass.game.isLoading)
            {
                return;
            }

            base.OnVisit();
            EClass._zone.parent = MageGuildZone;
        }
    }
}
