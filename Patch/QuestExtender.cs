using System;
using System.Reflection;

namespace RandomQuestExpantion.Patch
{
    static class QuestExtender
    {
        // Questに独自イベントハンドラを実装する
        public static void OnInvestMod(this Quest quest)
        {
            string targetmethod = "OnInvest";
            InvokeMethod(quest, targetmethod);
        }

        public static void OnShippedMod(this Quest quest, int priceAmount)
        {
            string targetmethod = "OnShipped";
            InvokeMethod(quest, targetmethod, new object[] { priceAmount });
        }

        public static void OnSoldSlaveMod(this Quest quest, Chara slave)
        {
            string targetmethod = "OnSoldSlave";
            InvokeMethod(quest, targetmethod, new object[] { slave });
        }

        public static void OnSoldMerchandiseMod(this Quest quest, Thing merchandise)
        {
            string targetmethod = "OnSoldMerchandise";
            InvokeMethod(quest, targetmethod, new object[] { merchandise });
        }

        public static void OnNefiaBeatenMod(this Quest quest, Chara boss)
        {
            string targetmethod = "OnNefiaBeaten";
            InvokeMethod(quest, targetmethod, new object[] { boss });
        }

        public static void InvokeMethod(in Quest quest, string targetMethod, object[] args = null)
        {
            Type type = quest.GetType();
            MethodInfo method = type.GetMethod(targetMethod, BindingFlags.Instance | BindingFlags.Public);

            if (method != null)
            {
                method.Invoke(quest, args);
            }
        }
    }
}
