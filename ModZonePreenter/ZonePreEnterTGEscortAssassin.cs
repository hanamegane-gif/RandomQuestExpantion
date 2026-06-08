namespace RandomQuestExpantion.ModZonePreenter
{
    class ZonePreEnterTGEscortAssassin : ZonePreEnterEscortAssassin
    {
        public ZonePreEnterTGEscortAssassin(string charaName) : base(charaName)
        {
            EscortTargetName = charaName;
        }
    }
}
