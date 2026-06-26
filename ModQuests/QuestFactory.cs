using System;
using System.Linq;
using System.Reflection;

namespace RandomQuestExpantion.ModQuests
{
    internal class QuestFactory
    {
        internal static Quest CreateQuestInstance(string questId, string idClientChara, Chara client)
        {
            Quest questInstance = null;
            var sourceRow = EClass.sources.quests.map[questId];
            string questType = sourceRow.type.IsEmpty("Quest");

            var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.FullName.EndsWith(questType));

            if (type == null)
            {
                RandomQuestExpantion.Log("questIdに該当するクエストがありません id:" + questId);
                return null;
            }

            questInstance = (Quest)Activator.CreateInstance(type);


            if (questInstance != null)
            {
                questInstance.id = questId;
                questInstance.person = new Person(idClientChara);
                if (questInstance is QuestDestZone { IsDeliver: not false } questDestZone)
                {
                    // クエスト期限に距離が考慮されないため配達先候補を調整する
                    // 単純に期限を伸ばすといつまでもクエストが残り続ける
                    var zone = Quest.ListDeliver().RandomItemWeighted(z => 15 - Math.Abs(Math.Abs(10 - z.tempDist) - z.tempDist));
                    questDestZone.SetDest(zone, zone.dictCitizen.Keys.RandomItem());
                }
                if (client != null)
                {
                    questInstance.SetClient(client);
                }
                questInstance.Init();
            }

            return questInstance;
        }
    }
}
