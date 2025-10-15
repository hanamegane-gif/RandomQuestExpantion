using UnityEngine;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventDuel : ZoneEventSubdue
    {
        public override bool WarnBoss => false;

        public override void OnVisit()
        {
            if (!EClass.game.isLoading)
            {
                int dangerLv = CalcZoneDangerLv();
                int numEnemies = CalcNumberOfEnemies();

                EClass._zone._dangerLv = dangerLv;

                for (int i = 0; i < numEnemies; i++)
                {
                    SpawnEnemy(dangerLv);
                }
                AggroEnemy(50);
                EClass._zone.SetBGM(102);
                max = enemies.Count;

                RandomQuestExpantion.Log(EClass._zone?.Name ?? "ない");
                RandomQuestExpantion.Log(EClass._zone.ParentZone?.Name ?? "ない");
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

        internal virtual int CalcZoneDangerLv()
        {
            return Mathf.Max(base.quest.DangerLv - 2, 1);
        }


        internal virtual int CalcNumberOfEnemies()
        { 
            return 1 + base.quest.difficulty / 2;
        }

        internal virtual void SpawnEnemy(int dangerLv)
        {
            Point spawnPoint = EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false, minRadius : 3);

            Chara enemy = CreateEnemy(dangerLv);

            EClass._zone.AddCard(enemy, spawnPoint);

            enemies.Add(enemy.uid);
        }

        internal virtual Chara CreateEnemy(int dangerLv)
        {
            int generateLv = Mathf.Max(dangerLv * 3 / 2, 5);
            int BPLv = Mathf.Max(dangerLv * 3 / 2, 5);

            CardBlueprint cardBlueprint = new CardBlueprint
            {
                rarity = Rarity.Legendary,
                lv = BPLv
            };
            CardBlueprint.Set(cardBlueprint);

            Chara createdChara = CharaGen.Create((EClass.rnd(10) == 0 ? "adv_fairy" : "adv"), generateLv);

            createdChara.c_bossType = BossType.Boss;
            createdChara.c_originalHostility = Hostility.Enemy;
            createdChara.hostility = Hostility.Enemy;

            return createdChara;
        }
    }
}
