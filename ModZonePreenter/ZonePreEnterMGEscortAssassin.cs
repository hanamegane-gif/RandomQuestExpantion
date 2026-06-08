namespace RandomQuestExpantion.ModZonePreenter
{
    class ZonePreEnterMGEscortAssassin : ZonePreEnterEscortAssassin
    {
        public ZonePreEnterMGEscortAssassin(string charaName) : base(charaName)
        {
            EscortTargetName = charaName;
        }
    }
}
