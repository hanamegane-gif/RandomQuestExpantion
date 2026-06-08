using Newtonsoft.Json;
using System;


namespace RandomQuestExpantion.ModNefia
{
    class ZoneRandomQuestNefiaFactory : Zone_RandomDungeonFactory, IQuestRandomNefia
    {
        [JsonProperty]
        private string ModZoneId = "";

        [JsonProperty]
        private string VanillaZoneId = "";

        public override string GetNewZoneID(int destLv)
        {
            return this.ModZoneId;
        }

        public void Init()
        {
            RevertToVanillaZoneId();
        }

        public void RevertToVanillaZoneId()
        {
            this.ModZoneId = String.Copy(this.id);
            this.VanillaZoneId = this.ModZoneId.Replace("byakko_mod_rqx_", "");
            this.id = String.Copy(this.VanillaZoneId);
        }
    }
}
