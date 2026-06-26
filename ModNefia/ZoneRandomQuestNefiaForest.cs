using Newtonsoft.Json;
using System;

namespace RandomQuestExpantion.ModNefia
{
    class ZoneRandomQuestNefiaForest : Zone_RandomDungeonForest, IQuestRandomNefia
    {
        public override string Name => "byakko_mod_nefia_rescue_prefix".lang() + Lang.space + ((idPrefix == 0) ? "" : (EClass.sources.zoneAffixes.map[idPrefix].GetName().ToTitleCase() + Lang.space)) + name.IsEmpty(source.GetText()) + NameSuffix;

        [JsonProperty]
        private string ModZoneId = "";

        [JsonProperty]
        private string VanillaZoneId = "";

        public override string GetNewZoneID(int destLv)
        {
            return this.ModZoneId;
        }

        public void RevertToVanillaZoneId()
        {
            this.ModZoneId = String.Copy(this.id);
            this.VanillaZoneId = this.ModZoneId.Replace("byakko_mod_rqx_", "");
            this.id = String.Copy(this.VanillaZoneId);
        }
    }
}
