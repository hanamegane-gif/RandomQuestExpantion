using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RandomQuestExpantion.General
{
    class ModMapPiece
    {
        public static PartialMap TryAddMapPiece(GenBounds targetBound, string pieceType, Action<PartialMap, GenBounds> onCreate = null)
        {
            var partialMap = ModMapPieceManager.GetMapPieceRandomOne(pieceType);
            if (partialMap == null)
            {
                return null;
            }

            bool flag = partialMap.dir == 1 || partialMap.dir == 3;

            var applyBound = targetBound.GetBounds(flag ? partialMap.h : partialMap.w, flag ? partialMap.w : partialMap.h, partialMap.ignoreBlock);
            if (applyBound == null)
            {
                return null;
            }

            partialMap.Apply(new Point(applyBound.x, applyBound.y), PartialMap.ApplyMode.Apply);
            onCreate?.Invoke(partialMap, applyBound);
            return partialMap;
        }
    }

    class ModMapPieceManager
    {
        const string __MOD_MAP_PIECE_DIR__ = "mappiece";

        internal static List<string> PieceTypeList { get; } = new List<string>
        {
            "fish",
            "herb",
            "crim_crop",
            "crim_factory",
        };

        internal class MapPieceSet
        {
            public string type;

            public List<string> pathList = new List<string>();

            public MapPieceSet(string _type, List<string> _pathList)
            {
                type = _type;
                pathList = _pathList;
            }
        }

        internal static bool Initialized;

        internal static List<MapPieceSet> PieceSet;

        internal static Dictionary<string, PartialMap> MapPieceCache = new Dictionary<string, PartialMap>();

        internal static PartialMap GetMapPieceRandomOne(string type)
        {
            var pieceSet = PieceSet.Where(a => a.type == type).FirstOrDefault();

            if (pieceSet == null || !pieceSet.pathList.Any())
            {
                return null;
            }

            string path = pieceSet.pathList.RandomItem();
            var partialMap = MapPieceCache.TryGetValue(path);
            if (partialMap == null)
            {
                partialMap = PartialMap.Load(path);
                MapPieceCache.Add(path, partialMap);
            }

            if (partialMap.allowRotate)
            {
                partialMap.dir = EClass.rnd(4);
            }

            partialMap.procedural = true;
            partialMap.ruinChance = 0f;
            return partialMap;
        }

        internal static void Init(in PluginInfo info)
        {
            if (Initialized)
            {
                return;
            }

            PieceSet = new List<MapPieceSet>();
            string mapPieceDir = Path.Combine(Path.GetDirectoryName(info.Location), __MOD_MAP_PIECE_DIR__);

            foreach (string type in PieceTypeList)
            {
                string dir = Path.Combine(mapPieceDir, type);
                string[] files = Directory.GetFiles(dir, "*.mp", SearchOption.AllDirectories);

                var pathes = new List<string>(); 
                foreach (string path in files)
                {
                    var directory = new FileInfo(path).Directory;
                    pathes.Add(path);
                }

                PieceSet.Add(new MapPieceSet(type, pathes));
            }

            Initialized = true;
        }

        internal void Reset()
        {
            Initialized = false;
        }
    }
}
