using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomQuestExpantion.Config
{
    class ModConfig
    {
        const string __CONFIG_FILE_NAME__ = "byakko.elin.RandomQuest.cfg";

        private static ConfigFile CustomConfig;
        private static ConfigEntry<int> _RewardPlatRate;
        private static ConfigEntry<bool> _EnableRuneVesselFeature;

        internal static int RewardPlatRate { get => _RewardPlatRate.Value; }

        internal static bool EnableRuneVesselFeature { get => _EnableRuneVesselFeature.Value; }

        internal static void LoadConfig()
        {
            string configPath = Path.Combine(Paths.ConfigPath, __CONFIG_FILE_NAME__);
            CustomConfig = new ConfigFile(configPath, true);

            _RewardPlatRate = CustomConfig.Bind
            (
                "General",
                "RewardPlatRate",
                100,
                "plat rate for specific quests reward(%), 0 is equivalent to vanilla.(100% is the balance assumed by the mod) : 一部依頼の報酬のプラチナ硬貨の倍率(%)、0にした場合はバニラと同量にします。(100%がModで想定しているバランスです)"
            );

            _EnableRuneVesselFeature = CustomConfig.Bind
            (
                "General",
                "EnableRuneVesselFeature",
                true,
                "if false, cancel gear genration which rune vessel enchanted.: falseにすると奪還依頼や交換所で生成される装備にルーンの器が付かなくなります。"
            );
        }
    }
}
