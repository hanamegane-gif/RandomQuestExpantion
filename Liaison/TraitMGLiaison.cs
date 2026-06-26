using System;
using System.Collections.Generic;

class TraitMGLiaison : TraitGuildLiaison
{
    internal static string QuestTag => "merchant";

    internal static string LiaisonId => "liaisonmg";

    internal static Dictionary<string, Tuple<int, int, int>> LiaisonPosDict => new Dictionary<string, Tuple<int, int, int>>
    {
        // X座標, Z座標, 人数
        { "kapul", new Tuple<int, int, int>(54, 75, 2) },
        { "tinkerCamp", new Tuple<int, int, int>(50, 53, 1) },
        { "olvina", new Tuple<int, int, int>(50, 42, 1) },
        { "aquli", new Tuple<int, int, int>(54, 55, 1) },
        { "yowyn", new Tuple <int, int, int>(40, 57, 1) },
        { "specwing", new Tuple <int, int, int>(50, 51, 2)  },
        { "village_exile", new Tuple <int, int, int>(55, 52, 0) },
        { "foxtown", new Tuple <int, int, int>(55, 46, 1) },
        { "foxtown_nefu", new Tuple <int, int, int>(56, 63, 1) },
        { "palmia", new Tuple <int, int, int>(73, 55, 2) },
        { "lothria", new Tuple <int, int, int>(47, 49, 1) },
        { "mysilia", new Tuple <int, int, int>(78, 78, 0) },
        { "derphy", new Tuple <int, int, int>(93, 70, 1) },
        { "lumiest", new Tuple <int, int, int>(61, 57, 1) },
        { "noyel", new Tuple <int, int, int>(43, 73, 1) },
    };
}
