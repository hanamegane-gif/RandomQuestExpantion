using RandomQuestExpantion.ModQuests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RandomQuestExpantion.ModQuests
{
    internal class QuestFactory
    {
        internal static Quest CreateQuestInstance(string questId)
        {
            Quest questInstance = null;
            string questType = EClass.sources.quests.map[questId].type.IsEmpty("Quest");

            switch (questType)
            {
                case "QuestDuel":
                    questInstance = new QuestDuel();
                    break;
                case "QuestDungeonAttack":
                    questInstance = new QuestDungeonAttack();
                    break;
                case "QuestDungeonRetrieve":
                    questInstance = new QuestDungeonRetrieve();
                    break;
                case "QuestFarmFieldWar":
                    questInstance = new QuestFarmFieldWar();
                    break;
                case "QuestHQFurniture":
                    questInstance = new QuestHQFurniture();
                    break;
                case "QuestHQMeal":
                    questInstance = new QuestHQMeal();
                    break;
                case "QuestUrbanIntrusion":
                    questInstance = new QuestUrbanIntrusion();
                    break;
                case "QuestUrbanBoss":
                    questInstance = new QuestUrbanBoss();
                    break;

                default:
                    RandomQuestExpantion.Log("questIdに該当するクエストがありません id:" + questId);
                    break;
            }
            return questInstance;
        }
    }
}
