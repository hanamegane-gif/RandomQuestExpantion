using System;
using System.Collections.Generic;

class TraitTGLiaison : TraitGuildLiaison
{
    internal static string QuestTag => "thief";

    internal static string LiaisonId => "liaisontg";

    internal static Dictionary<string, Tuple<int, int, int>> LiaisonPosDict => new Dictionary<string, Tuple<int, int, int>>
    {
        // X座標, Z座標, 人数
        { "kapul", new Tuple<int, int, int>(76, 68, 1) },
        { "tinkerCamp", new Tuple<int, int, int>(57, 64, 1) },
        { "olvina", new Tuple<int, int, int>(64, 45, 1) },
        { "aquli", new Tuple <int, int, int>(36, 36, 1) },
        { "yowyn", new Tuple <int, int, int>(50, 39, 1) },
        { "specwing", new Tuple <int, int, int>(61, 46, 1) },
        { "village_exile", new Tuple <int, int, int>(51, 33, 0) },
        { "foxtown", new Tuple <int, int, int>(61, 59, 2) },
        { "foxtown_nefu", new Tuple <int, int, int>(40, 59, 1) },
        { "palmia", new Tuple <int, int, int>(28, 55, 1) },
        { "lothria", new Tuple <int, int, int>(59, 40, 1) },
        { "mysilia", new Tuple <int, int, int>(75, 46, 1) },
        { "derphy", new Tuple <int, int, int>(68, 70, 0) },
        { "lumiest", new Tuple <int, int, int>(58, 105, 2) },
        { "noyel", new Tuple <int, int, int>(90, 52, 1) },
    };
}
