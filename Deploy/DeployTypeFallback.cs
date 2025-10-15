using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RandomQuestExpantion.Patch
{
    class DeployTypeFallback
    {
        internal static void DeployTypeFallbackSetting()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var targetNamespaceList = new List<string> 
            { 
                "RandomQuestExpantion.ModQuestEvent", 
                "RandomQuestExpantion.ModQuests.Common",
                "RandomQuestExpantion.ModQuests.FighterGuild",
                "RandomQuestExpantion.ModQuests.MageGuild",
                "RandomQuestExpantion.ModQuests.MerchantGuild",
                "RandomQuestExpantion.ModQuests.ThiefGuild",
                "RandomQuestExpantion.ModQuestTask", 
                "RandomQuestExpantion.ModQuestZoneInstance" 
            };
            var typeFallbackSetting = ReadTypeFallbackSetting();
            bool shouldFileUpdate = false;

            foreach (var targetNamespace in targetNamespaceList)
            {
                var classes = assembly.GetTypes().Where(t => t.IsClass && t.Namespace == targetNamespace);

                foreach (var type in classes)
                {
                    string className = type.Name;
                    string baseClass = "";


                    // フォールバックが必要なのはこの4種
                    if (typeof(Quest).IsAssignableFrom(type))
                    {
                        // クエストはタイプフォールバックを効かせても結局保存されたidから依頼文章などを復元しようとするのでもきゅもきゅクエストにしないといけない
                        baseClass = "QuestDummy";
                    }
                    else if (typeof(QuestTask).IsAssignableFrom(type))
                    {
                        baseClass = "QuestTask";
                    }
                    else if (typeof(ZoneEvent).IsAssignableFrom(type))
                    {
                        baseClass = "ZoneEvent";
                    }
                    else if (typeof(ZoneInstance).IsAssignableFrom(type))
                    {
                        baseClass = "ZoneInstance";
                    }

                    if (baseClass == "")
                    {
                        continue;
                    }

                    // "「アセンブリ名」,「名前空間 + クラス名」, 「フォールバック先Elinクラス名」"
                    string fallbackRowString = assembly.GetName().Name + "," + (targetNamespace + "." + className) + "," + baseClass;

                    if (!typeFallbackSetting.Contains(fallbackRowString))
                    {
                        typeFallbackSetting.Add(fallbackRowString);
                        shouldFileUpdate = true;
                    }
                }
            }


            if (shouldFileUpdate)
            {
                string path = Path.Combine(CorePath.RootData, "type_resolver.txt");
                IO.SaveTextArray(path, typeFallbackSetting.ToArray());
            }
        }

        private static List<string> ReadTypeFallbackSetting()
        {
            string text = "type_resolver.txt";
            string[] array = new string[0];
            if (File.Exists(CorePath.RootData + text))
            {
                array = IO.LoadTextArray(CorePath.RootData + text);
            }
            else
            {
                array = new string[2] { "TrueArena,ArenaWaveEvent,ZoneEvent", "Elin-GeneRecombinator,Elin_GeneRecombinator.IncubationSacrifice,Chara" };
                IO.SaveTextArray(CorePath.RootData + text, array);
            }

            return array.ToList();
        }
    }
}
