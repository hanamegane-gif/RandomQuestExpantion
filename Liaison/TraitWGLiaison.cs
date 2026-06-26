using System;
using System.Collections.Generic;

class TraitWGLiaison : TraitGuildLiaison
{
    internal static string QuestTag => "mage";

    internal static string LiaisonId => "liaisonwg";

    internal static Dictionary<string, Tuple<int, int, int>> LiaisonPosDict => new Dictionary<string, Tuple<int, int, int>>
    {
        // X座標, Z座標, 人数
        { "kapul", new Tuple<int, int, int>(50, 95, 1) },
        { "tinkerCamp", new Tuple<int, int, int>(64, 51, 1) },
        { "olvina", new Tuple<int, int, int>(53, 53, 2) },
        { "aquli", new Tuple<int, int, int>(48, 53, 1) },
        { "yowyn", new Tuple<int, int, int>(33, 41, 1) },
        { "specwing", new Tuple<int, int, int>(58, 59, 1) },
        { "village_exile", new Tuple<int, int, int>(39, 63, 0) },
        { "foxtown", new Tuple<int, int, int>(39, 58, 2) },
        { "foxtown_nefu", new Tuple<int, int, int>(58, 46, 1) },
        { "palmia", new Tuple<int, int, int>(54, 44, 1) },
        { "lothria", new Tuple<int, int, int>(39, 50, 1) },
        { "mysilia", new Tuple<int, int, int>(50, 70, 1) },
        { "derphy", new Tuple<int, int, int>(71, 79, 1) },
        { "lumiest", new Tuple<int, int, int>(41, 92, 0) },
        { "noyel", new Tuple<int, int, int>(75, 87, 2) },
    };
}
