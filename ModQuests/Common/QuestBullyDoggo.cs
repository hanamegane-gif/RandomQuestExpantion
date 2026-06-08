using RandomQuestExpantion.Config;
using RandomQuestExpantion.ModQuestTask;
using System.Linq;
using UnityEngine;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestBullyDoggo : QuestHunt
    {
        public override void OnInit()
        {
            SetTask(new TaskBullyDoggo());
        }

        public override void OnStart()
        {
            var spawnedDoggo = SpawnDoggo();
            (task as TaskBullyDoggo).SetUIDDoggo(spawnedDoggo.uid);
        }

        public override int GetRewardPlat(int money)
        {
            int bonusPlat = (ModConfig.RewardPlatRate > 0) ? EClass.curve(bonusMoney / 400, 6, 10, 75) * ModConfig.RewardPlatRate / 100 : EClass.rndHalf((int)Mathf.Sqrt(money / 200));
            return 1 + EClass.rnd(2) + bonusPlat;
        }

        internal Chara SpawnDoggo()
        {
            var doggoSource = EClass.sources.charas.rows.Where(r => r.id == "wolfguy").First();
            int originalLv = doggoSource.LV;
            var cardBlueprint = new CardBlueprint
            {
                rarity = Rarity.Legendary,
                lv = originalLv,
            };
            CardBlueprint.Set(cardBlueprint);

            var spawnedDoggo = CharaGen.Create(doggoSource.id, originalLv);
            spawnedDoggo.hostility = Hostility.Neutral;

            EClass._zone.AddCard(spawnedDoggo, EClass._map.bounds.GetRandomEdge().GetNearestPoint(allowBlock: false, allowChara: false, minRadius: 10));

            ActEffect.Proc(EffectId.Buff, 1000, BlessedState.Normal, spawnedDoggo, null, new ActRef
            {
                n1 = "ConTransmuteHuman"
            });

            // 見つけやすいように血と頭装備の目印をつける
            spawnedDoggo.body.GetEquippedThing(SLOT.head)?.Destroy();
            var thing = spawnedDoggo.EQ_ID("mask_jason");
            spawnedDoggo.AddBlood(40);

            return spawnedDoggo;
        }
    }
}
