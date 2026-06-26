using Newtonsoft.Json;
using System;

namespace RandomQuestExpantion.ModNefia
{
    class ZoneRandomQuestNefia : Zone_RandomDungeon, IQuestRandomNefia
    {
        public override string Name => "byakko_mod_nefia_rescue_prefix".lang() + Lang.space + ((idPrefix == 0) ? "" : (EClass.sources.zoneAffixes.map[idPrefix].GetName().ToTitleCase() + Lang.space)) + name.IsEmpty(source.GetText()) + NameSuffix;

        [JsonProperty]
        private string ModZoneId = "";

        [JsonProperty]
        private string VanillaZoneId = "";

        // これは階層移動時に生成されるZoneのidとして使われる
        // Mod外した際のセーブデータ破壊を防ぐため保存するidはバニラのidである必要がある
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
