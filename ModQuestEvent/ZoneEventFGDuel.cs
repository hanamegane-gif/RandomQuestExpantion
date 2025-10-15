using UnityEngine;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventFGDuel : ZoneEventDuel
    {
        public override void OnVisit()
        {
            base.OnVisit();
            if (!EClass.game.isLoading)
            {
                // ギルドマップから受注すると設定してやらないとParentZoneが存在しないので戻ってこれなくなる
                EClass._zone.parent = FighterGuildZone;
            }
        }

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

            createdChara.c_bossType = BossType.Boss;
            createdChara.c_originalHostility = Hostility.Enemy;
            createdChara.hostility = Hostility.Enemy;

            return createdChara;
        }
    }
}
