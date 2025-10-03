using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Lang;
using static UnityEngine.UI.GridLayoutGroup;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventDuel : ZoneEventSubdue
    {
        public override bool WarnBoss => false;

        public override void OnVisit()
        {
            if (!EClass.game.isLoading)
            {
                int dangerLv = Mathf.Max(base.quest.DangerLv - 2, 1);
                int numAdventurer = 1 + base.quest.difficulty / 2;

                EClass._zone._dangerLv = dangerLv;

                for (int i = 0; i < numAdventurer; i++)
                {
                    SpawnBoss(dangerLv);
                }
                AggroEnemy(50);
                EClass._zone.SetBGM(102);
                max = enemies.Count;
            }
        }

        public override void OnCharaDie(Chara c)
        {
            CheckClear();
        }

        public override void _OnTickRound()
        {
            AggroEnemy();
            CheckClear();
        }

        private void SpawnBoss(int dangerLv)
        {
            Point spawnPoint = EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false, minRadius : 3);

            Chara chara = SpawnAdventurer(dangerLv);
            Hostility hostility2 = (chara.c_originalHostility = Hostility.Enemy);
            chara.hostility = hostility2;
            enemies.Add(chara.uid);
        }

        private Chara SpawnAdventurer(int dangerLv)
        {
            Point spawnPosition = EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false, minRadius: 3);

            int generateLv = Mathf.Max(dangerLv * 3 / 2, 5);
            int BPLv = Mathf.Max(dangerLv * 3 / 2, 5);

            CardBlueprint cardBlueprint = new CardBlueprint
            {
                rarity = Rarity.Legendary,
                lv = BPLv
            };
            CardBlueprint.Set(cardBlueprint);

            Chara createdChara = CharaGen.Create((EClass.rnd(10) == 0 ? "adv_fairy" : "adv"), generateLv);
            createdChara.SetLv(Mathf.Max(generateLv, 5));
            createdChara.TryRestock(true);

            EClass._zone.AddCard(createdChara, spawnPosition);

            createdChara.c_bossType = BossType.Boss;
            Hostility c_originalHostility = Hostility.Enemy;
            createdChara.c_originalHostility = c_originalHostility;

            return createdChara;
        }
    }
}
