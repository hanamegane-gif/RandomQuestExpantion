using UnityEngine;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventFGDuel : ZoneEventDuel
    {
        internal override int CalcNumberOfEnemies()
        {
            return 2 + base.quest.difficulty;
        }

        internal override Chara CreateEnemy(int dangerLv)
        {
            int generateLv = Mathf.Max(dangerLv * 3 / 2, 5);
            int BPLv = Mathf.Max(dangerLv * 3 / 2, 5);

            CardBlueprint cardBlueprint = new CardBlueprint
            {
                rarity = Rarity.Legendary,
                lv = BPLv
            };
            CardBlueprint.Set(cardBlueprint);

            Chara createdChara = CharaGen.Create((EClass.rnd(2) == 0 ? "guild_warrior" : "merc_warrior"), generateLv);

            int charaLv = (dangerLv < 100) ? dangerLv * 4 / 5 :
                          (dangerLv < 300) ? dangerLv :
                          (dangerLv < 500) ? dangerLv * 5 / 4 : generateLv;
            createdChara.SetLv(charaLv);

            createdChara.c_bossType = BossType.Boss;
            createdChara.c_originalHostility = Hostility.Enemy;
            createdChara.hostility = Hostility.Enemy;

            return createdChara;
        }
    }
}
