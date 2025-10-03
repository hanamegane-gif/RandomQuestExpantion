using HarmonyLib;
using RandomQuestExpantion.ModQuests;
using RandomQuestExpantion.ModQuests.Common;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomQuestExpantion.Patch
{
    [HarmonyPatch]
    class QuestPatch
    {
        [HarmonyPatch(typeof(Quest), nameof(Quest.Create)), HarmonyPrefix]
        internal static bool QuestCreatePatch(ref Quest __result, ref string _id, ref string _idPerson, ref Chara c)
        {
            if (!_id.Contains("byakko_mod"))
            {
                return true;
            }

            Quest quest = QuestFactory.CreateQuestInstance(_id);
            quest.id = _id;
            quest.person = new Person(_idPerson);
            if (quest is QuestDestZone { IsDeliver: not false } questDestZone)
            {
                Zone zone = Quest.ListDeliver().RandomItem();
                questDestZone.SetDest(zone, zone.dictCitizen.Keys.RandomItem());
            }
            if (c != null)
            {
                quest.SetClient(c);
            }
            quest.Init();
            __result = quest;
            return false;
        }
    }
}
