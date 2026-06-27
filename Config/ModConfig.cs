using BepInEx;
using BepInEx.Configuration;
using System.IO;

namespace RandomQuestExpantion.Config
{
    class ModConfig
    {
        const string __CONFIG_FILE_NAME__ = "byakko.elin.RandomQuest.cfg";

        private static ConfigFile CustomConfig;
        private static ConfigEntry<int> _RewardPlatRate;
        private static ConfigEntry<int> _MaxQuestLimit;
        private static ConfigEntry<bool> _EnableRuneVesselFeature;
        private static ConfigEntry<bool> _EnableDangerousDoggo;

        internal static int RewardPlatRate { get => _RewardPlatRate?.Value ?? 100; }

        internal static int MaxQuestLimit { get => _MaxQuestLimit?.Value ?? 12; }

        internal static bool EnableRuneVesselFeature { get => _EnableRuneVesselFeature?.Value ?? true; }

        internal static bool EnableDangerousDoggo { get => _EnableDangerousDoggo?.Value ?? false; }

        internal static void LoadConfig()
        {
            string configPath = Path.Combine(Paths.ConfigPath, __CONFIG_FILE_NAME__);
            CustomConfig = new ConfigFile(configPath, true);

            _RewardPlatRate = CustomConfig.Bind
            (
                "General",
                "RewardPlatRate",
                100,
                "Plat rate for specific quests reward(%), 0 is equivalent to vanilla.(100% is the balance assumed by the mod) : 一部依頼の報酬のプラチナ硬貨の倍率(%)、0にした場合はバニラと同量にします。(100%がModで想定しているバランスです)"
            );

            _MaxQuestLimit = CustomConfig.Bind
            (
                "General",
                "MaxQuestLimit",
                12,
                "Maximum number of quests can be accepted simultaneously. : 同時に受けられる依頼数の上限。"
            );

            _EnableRuneVesselFeature = CustomConfig.Bind
            (
                "General",
                "EnableRuneVesselFeature",
                true,
                "If false, cancel gear genration which rune vessel enchanted.: falseにすると依頼や交換所で生成される装備にルーンの器が付かなくなります。"
            );

            _EnableDangerousDoggo = CustomConfig.Bind
            (
                "General",
                "EnableDangerousDoggo",
                false,
                "If true, removes the Lv cap for werewolves in Werewolf Hunting quests.: trueにすると人狼狩りクエストで生成される人狼のLv上限を解除します。"
            );
        }
    }
}
