using BepInEx;
using System.IO;

namespace RandomQuestExpantion.Patch
{
    class ImportExcelPatch
    {
        internal static void execImportQuests(in PluginInfo info)
        {
            var dir = Path.GetDirectoryName(info.Location);
            var excel = Path.Combine(dir, "import/Quests.xlsx");
            var sources = Core.Instance.sources;
            ModUtil.ImportExcel(excel, "Quests", sources.quests);
        }

        internal static void execImportLanguages(in PluginInfo info)
        {
            var dir = Path.GetDirectoryName(info.Location);
            var excel = Path.Combine(dir, "import/LangGeneral.xlsx");
            var sources = Core.Instance.sources;
            ModUtil.ImportExcel(excel, "LangGeneral", sources.langGeneral);
        }

        internal static void execImportZones(in PluginInfo info)
        {
            var dir = Path.GetDirectoryName(info.Location);
            var excel = Path.Combine(dir, "import/Zones.xlsx");
            var sources = Core.Instance.sources;
            ModUtil.ImportExcel(excel, "Zones", sources.zones);
        }

        internal static void execImportThings(in PluginInfo info)
        {
            var dir = Path.GetDirectoryName(info.Location);
            var excel = Path.Combine(dir, "import/Things.xlsx");
            var sources = Core.Instance.sources;
            ModUtil.ImportExcel(excel, "Things", sources.things);
        }
    }
}
