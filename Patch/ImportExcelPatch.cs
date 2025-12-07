using BepInEx;
using System.IO;

namespace RandomQuestExpantion.Patch
{
    // sourceインポートを行う
    // シート名をローカライズMod側と統一しないとローカライズが壊れるので注意、簡体字ローカライズのシート名を参照すること
    class ImportExcelPatch
    {
        const string __MOD_SOURCE_DIR__ = "LangMod/JP";

        internal static void ExecImportQuests(in PluginInfo info)
        {
            var dir = Path.Combine(Path.GetDirectoryName(info.Location), __MOD_SOURCE_DIR__);
            var excel = Path.Combine(dir, "Quests.xlsx");
            var sources = Core.Instance.sources;
            ModUtil.ImportExcel(excel, "Quest", sources.quests);
        }

        internal static void ExecImportLanguages(in PluginInfo info)
        {
            var dir = Path.Combine(Path.GetDirectoryName(info.Location), __MOD_SOURCE_DIR__);
            var excel = Path.Combine(dir, "LangGeneral.xlsx");
            var sources = Core.Instance.sources;
            ModUtil.ImportExcel(excel, "General", sources.langGeneral);
        }

        internal static void ExecImportZones(in PluginInfo info)
        {
            var dir = Path.Combine(Path.GetDirectoryName(info.Location), __MOD_SOURCE_DIR__);
            var excel = Path.Combine(dir, "Zones.xlsx");
            var sources = Core.Instance.sources;
            ModUtil.ImportExcel(excel, "Zone", sources.zones);
        }

        internal static void ExecImportThings(in PluginInfo info)
        {
            var dir = Path.Combine(Path.GetDirectoryName(info.Location), __MOD_SOURCE_DIR__);
            var excel = Path.Combine(dir, "Things.xlsx");
            var sources = Core.Instance.sources;
            ModUtil.ImportExcel(excel, "Thing", sources.things);
        }
    }
}
