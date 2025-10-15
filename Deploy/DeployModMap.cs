using BepInEx;
using System;
using System.IO;
using System.Security.Cryptography;

namespace RandomQuestExpantion.Patch
{
    class DeployModMap
    {
        internal static bool DeployModMaps(in PluginInfo info)
        {
            string sourceDir = Path.Combine(Path.GetDirectoryName(info.Location), "maps") ;
            string targetDir = CorePath.ZoneSave;

            if (!Directory.Exists(sourceDir))
            {
                RandomQuestExpantion.Log("Modこわれる＾～");
                return false;
            }

            foreach (var sourceFilePath in Directory.GetFiles(sourceDir))
            {
                string mapFileName = Path.GetFileName(sourceFilePath);
                string targetFilePath = Path.Combine(targetDir, mapFileName);

                if (File.Exists(targetFilePath))
                {
                    string sourceHash = ComputeMD5(sourceFilePath);
                    string targetHash = ComputeMD5(targetFilePath);

                    if (sourceHash != targetHash)
                    {
                        File.Copy(sourceFilePath, targetFilePath, true);
                    }
                }
                else
                {
                    File.Copy(sourceFilePath, targetFilePath);
                }
            }

            return true;
        }

        private static string ComputeMD5(string filePath)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
