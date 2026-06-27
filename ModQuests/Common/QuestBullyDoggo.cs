using Newtonsoft.Json;
using RandomQuestExpantion.Config;
using RandomQuestExpantion.ModQuestEvent;
using RandomQuestExpantion.ModQuestTask;
using System.Linq;
using UnityEngine;

namespace RandomQuestExpantion.ModQuests.Common
{
    public class QuestBullyDoggo : QuestHunt
    {
        [JsonProperty]
        public int UIDDoggo = -1;

        public override int BaseMoney => source.money + EClass.curve((!ModConfig.EnableDangerousDoggo && dangerLv > 2000) ? 2000 : dangerLv, 20, 15) * 10;

        public override void OnInit()
        {
            SetTask(new TaskBullyDoggo());
        }

        public override void OnStart()
        {
            var doggo = SpawnDoggo();

            UIDDoggo = doggo.uid;
            (task as TaskBullyDoggo).SetUIDDoggo(doggo.uid);

            var woofEvent = new ZoneEventBullyDoggo();
            woofEvent.SetOwnerQuest(this);
            woofEvent.SetUIDDoggo(doggo.uid);
            EClass._zone.events.Add(woofEvent);
        }

        public override void OnFail()
        {
            EClass._zone.map.charas.Where(c => c.uid == UIDDoggo).FirstOrDefault()?.Destroy();
        }

        public override int GetRewardPlat(int money)
        {
            int bonusPlat = (ModConfig.RewardPlatRate > 0) ? EClass.curve(money / 800, 6, 10, 75) * ModConfig.RewardPlatRate / 100 : EClass.rndHalf((int)Mathf.Sqrt(money / 200));
            return 1 + EClass.rnd(2) + bonusPlat;
        }

        internal Chara SpawnDoggo()
        {
            var doggoSource = EClass.sources.charas.rows.Where(r => r.id == "wolfguy").First();
            int doggoLv = this.dangerLv * 5 / 4;

            // 高速で街を壊滅させるのを野放しにするのは危険すぎたためオプション化
            if (!ModConfig.EnableDangerousDoggo && doggoLv > 2500)
            {
                doggoLv = 2500;
            }

            var cardBlueprint = new CardBlueprint
            {
                rarity = Rarity.Legendary,
                lv = doggoLv,
            };
            CardBlueprint.Set(cardBlueprint);

            var spawnedDoggo = CharaGen.Create(doggoSource.id, doggoLv);
            spawnedDoggo.SetLv(doggoLv);
            spawnedDoggo.hostility = Hostility.Neutral;

            EClass._zone.AddCard(spawnedDoggo, EClass._map.bounds.GetRandomPoint().GetNearestPoint(allowBlock: false, allowChara: false, minRadius: 1));

            ActEffect.Proc(EffectId.Buff, 1000, BlessedState.Normal, spawnedDoggo, null, new ActRef
            {
                n1 = "ConTransmuteHuman"
            });

            // 見つけやすいように血と頭装備の目印をつける
            // ヒント無しだと物凄く見つけづらい
            spawnedDoggo.body.GetEquippedThing(SLOT.head)?.Destroy();
            var thing = spawnedDoggo.EQ_ID("mask_jason");
            spawnedDoggo.AddBlood(20);

            return spawnedDoggo;
        }
    }
}
