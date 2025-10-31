using RandomQuestExpantion.ModQuests.Common;
using RandomQuestExpantion.ModQuests.FighterGuild;
using RandomQuestExpantion.ModQuests.MageGuild;
using RandomQuestExpantion.ModQuests.MerchantGuild;
using RandomQuestExpantion.ModQuests.ThiefGuild;

namespace RandomQuestExpantion.ModQuests
{
    internal class QuestFactory
    {
        internal static Quest CreateQuestInstance(string questId, string idClientChara, Chara client)
        {
            Quest questInstance = null;
            string questType = EClass.sources.quests.map[questId].type.IsEmpty("Quest");

            switch (questType)
            {
                case "QuestCrimFactory":
                    questInstance = new QuestCrimFactory();
                    break;
                case "QuestDuel":
                    questInstance = new QuestDuel();
                    break;
                case "QuestDungeonAttack":
                    questInstance = new QuestDungeonAttack();
                    break;
                case "QuestDungeonRetrieve":
                    questInstance = new QuestDungeonRetrieve();
                    break;
                case "QuestFish":
                    questInstance = new QuestFish();
                    break;
                case "QuestFarmFieldWar":
                    questInstance = new QuestFarmFieldWar();
                    break;
                case "QuestHerbHarvest":
                    questInstance = new QuestHerbHarvest();
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
                case "QuestFGDefence":
                    questInstance = new QuestFGDefence();
                    break;
                case "QuestFGDuel":
                    questInstance = new QuestFGDuel();
                    break;
                case "QuestFGDungeonAttack":
                    questInstance = new QuestFGDungeonAttack();
                    break;
                case "QuestFGHunt":
                    questInstance = new QuestFGHunt();
                    break;
                case "QuestFGSubdue":
                    questInstance = new QuestFGSubdue();
                    break;
                case "QuestFGStrongerHunt":
                    questInstance = new QuestFGStrongerHunt();
                    break;
                case "QuestFGWithdrawal":
                    questInstance = new QuestFGWithdrawal();
                    break;
                case "QuestFGYeekHunt":
                    questInstance = new QuestFGYeekHunt();
                    break;
                case "QuestMGDefence":
                    questInstance = new QuestMGDefence();
                    break;
                case "QuestMGDeliver":
                    questInstance = new QuestMGDeliver();
                    break;
                case "QuestMGEscort":
                    questInstance = new QuestMGEscort();
                    break;
                case "QuestMGFlyer":
                    questInstance = new QuestMGFlyer();
                    break;
                case "QuestMGHunt":
                    questInstance = new QuestMGHunt();
                    break;
                case "QuestMGInvest":
                    questInstance = new QuestMGInvest();
                    break;
                case "QuestMGSales":
                    questInstance = new QuestMGSales();
                    break;
                case "QuestMGShipping":
                    questInstance = new QuestMGShipping();
                    break;
                case "QuestTGCrimFactory":
                    questInstance = new QuestTGCrimFactory();
                    break;
                case "QuestTGDeliver":
                    questInstance = new QuestTGDeliver();
                    break;
                case "QuestTGDefence":
                    questInstance = new QuestTGDefence();
                    break;
                case "QuestTGDungeonRetrieve":
                    questInstance = new QuestTGDungeonRetrieve();
                    break;
                case "QuestTGSalesStolen":
                    questInstance = new QuestTGSalesStolen();
                    break;
                case "QuestTGSlaver":
                    questInstance = new QuestTGSlaver();
                    break;
                case "QuestWGDefence":
                    questInstance = new QuestWGDefence();
                    break;
                case "QuestWGDuel":
                    questInstance = new QuestWGDuel();
                    break;
                case "QuestWGDungeonRetrieve":
                    questInstance = new QuestWGDungeonRetrieve();
                    break;
                case "QuestWGHerbHarvest":
                    questInstance = new QuestWGHerbHarvest();
                    break;
                case "QuestWGHQPotion":
                    questInstance = new QuestWGHQPotion();
                    break;
                case "QuestWGSubdueGhost":
                    questInstance = new QuestWGSubdueGhost();
                    break;

                default:
                    RandomQuestExpantion.Log("questIdに該当するクエストがありません id:" + questId);
                    break;
            }

            if (questInstance != null)
            {
                questInstance.id = questId;
                questInstance.person = new Person(idClientChara);
                if (questInstance is QuestDestZone { IsDeliver: not false } questDestZone)
                {
                    Zone zone = Quest.ListDeliver().RandomItem();
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
