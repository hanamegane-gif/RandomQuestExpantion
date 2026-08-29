using HarmonyLib;
using RandomQuestExpantion.Guilpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomQuestExpantion.Patch
{
    // ネフィア生成時にラーナ周辺のアクセス不能地域を弾く
    [HarmonyPatch]
    internal class EloMapPatch
    {
        // 経路探索アルゴリズムを書くのがめんどくさいのでラーナ付近だけ潰す
        private static readonly HashSet<Point> AccessDeniedZones = new HashSet<Point>
        {
            new Point(47, -45),
            new Point(49, -47),
            new Point(49, -48),
            new Point(48, -47),
            new Point(48, -48),
            new Point(48, -49),
            new Point(43, -44),
            new Point(43, -43),
            new Point(42, -44),
            new Point(45, -51),
            new Point(47, -52),
            new Point(49, -52),
            new Point(49, -51),
            new Point(50, -50),
        };

        [HarmonyPatch(typeof(EloMap), nameof(EloMap.CanBuildSite)), HarmonyPrefix]
        public static bool CanBuildSitePatch(ref bool __result, EloMap __instance, int gx, int gy, int radius, ElomapSiteType type)
        {
            if (type == ElomapSiteType.Nefia)
            {
                var point = new Point(gx, gy);

                if (AccessDeniedZones.Contains(point))
                {
                    __result = false;
                    return false;
                }
            }

            return true;
        }
    }
}
