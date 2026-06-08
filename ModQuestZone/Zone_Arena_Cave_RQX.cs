using System.IO;
using System.Linq;

class Zone_Arena_Cave_RQX : Zone_Arena
{
    const string __MOD_MAP_DIR__ = "maps";

    private string MapDirectory => Path.Combine(Path.GetDirectoryName(RandomQuestExpantion.RandomQuestExpantion.DLLInfo.Location), __MOD_MAP_DIR__);

    public override string pathExport => Path.Combine(MapDirectory, this.source.idFile.First() + "1.z");
}