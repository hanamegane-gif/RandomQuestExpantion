using System;
using System.Reflection;

namespace RandomQuestExpantion.Patch
{
    static class QuestTaskExtender
    {
        // QuestTaskに独自イベントハンドラを実装する
        public static void OnInvestMod(this QuestTask task)
        {
            string targetmethod = "OnInvest";
            InvokeMethod(task, targetmethod);
        }

        public static void OnShippedMod(this QuestTask task, int priceAmount)
        {
            string targetmethod = "OnShipped";
            InvokeMethod(task, targetmethod, new object[] { priceAmount });
        }

        public static void OnSoldSlaveMod(this QuestTask task, Chara slave)
        {
            string targetmethod = "OnSoldSlave";
            InvokeMethod(task, targetmethod, new object[] { slave });
        }

        public static void OnSoldMerchandiseMod(this QuestTask task, Thing merchandise)
        {
            string targetmethod = "OnSoldMerchandise";
            InvokeMethod(task, targetmethod, new object[] { merchandise });
        }

        public static void InvokeMethod(in QuestTask task, string targetMethod, object[] args = null)
        {
            Type type = task.GetType();
            MethodInfo method = type.GetMethod(targetMethod, BindingFlags.Instance | BindingFlags.Public);

            if (method != null)
            {
                method.Invoke(task, args);
            }
        }
    }
}
