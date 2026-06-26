using HarmonyLib;
using System;
using System.Reflection;

namespace RandomQuestExpantion.DayBreak
{
    static class QuestPatchup
    {
        private static FieldInfo BonusMoneyField => AccessTools.Field(typeof(Quest), "bonusMoney");

        internal static long GetBonusMoney(this Quest quest)
        {
            object value = BonusMoneyField.GetValue(quest);

            return value switch
            {
                int i => i,
                long l => l,
                _ => throw new NotSupportedException()
            };
        }

        internal static void SetBonusMoney(this Quest quest, long value)
        {
            if (BonusMoneyField.FieldType == typeof(int))
            {
                BonusMoneyField.SetValue(quest, checked((int)value));
            }
            else if (BonusMoneyField.FieldType == typeof(long))
            {
                BonusMoneyField.SetValue(quest, value);
            }
        }
    }
}
