using System;
using System.Collections.Generic;

class TraitFGLiaison : TraitGuildLiaison
{
    internal static string QuestTag => "fighter";

    internal static string LiaisonId => "liaisonfg";

    internal static Dictionary<string, Tuple<int, int, int>> LiaisonPosDict => new Dictionary<string, Tuple<int, int, int>>
    {
        // X座標, Z座標, 人数
        { "kapul", new Tuple<int, int, int>(61, 96, 0) },
        { "tinkerCamp", new Tuple<int, int, int>(56, 43, 1) },
        { "olvina", new Tuple<int, int, int>(34, 59, 1) },
        { "aquli", new Tuple<int, int, int>(58, 64, 2) },
        { "yowyn", new Tuple<int, int, int>(46, 49, 1) },
        { "specwing", new Tuple<int, int, int>(52, 42, 1) },
        { "village_exile", new Tuple<int, int, int>(35, 52, 0) },
        { "foxtown", new Tuple<int, int, int>(39, 40, 1) },
        { "foxtown_nefu", new Tuple<int, int, int>(33, 39, 2) },
        { "palmia", new Tuple<int, int, int>(70, 69, 1) },
        { "lothria", new Tuple<int, int, int>(60, 57, 2) },
        { "mysilia", new Tuple<int, int, int>(46, 100, 1) },
        { "derphy", new Tuple<int, int, int>(58, 71, 1) },
        { "lumiest", new Tuple<int, int, int>(78, 62, 1) },
        { "noyel", new Tuple<int, int, int>(37, 58, 1) },
    };
}
