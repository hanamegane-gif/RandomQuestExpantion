using BepInEx;
using System.IO;

namespace RandomQuestExpantion.Patch
{
    class ImportExcelPatch
    {
        internal static void execImportQuests(PluginInfo info)
        {
            var dir = Path.GetDirectoryName(info.Location);
            var excel = Path.Combine(dir, "Quests.xlsx");
            var sources = Core.Instance.sources;
            ModUtil.ImportExcel(excel, "Quests", sources.quests);
        }

        internal static void execImportLanguages(PluginInfo info)
        {
            var dir = Path.GetDirectoryName(info.Location);
            var excel = Path.Combine(dir, "LangGeneral.xlsx");
            var sources = Core.Instance.sources;
            ModUtil.ImportExcel(excel, "LangGeneral", sources.langGeneral);
        }

        internal static void execImportZones(PluginInfo info)
        {
            var dir = Path.GetDirectoryName(info.Location);
            var excel = Path.Combine(dir, "Zones.xlsx");
            var sources = Core.Instance.sources;
            ModUtil.ImportExcel(excel, "Zones", sources.zones);
        }
    }
}
