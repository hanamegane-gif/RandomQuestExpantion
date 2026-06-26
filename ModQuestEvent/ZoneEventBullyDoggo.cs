using Newtonsoft.Json;
using RandomQuestExpantion.General;
using RandomQuestExpantion.ModQuests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RandomQuestExpantion.ModQuestEvent
{
    // 人狼狩りクエストでヒント演出を出すためZoneEvent
    internal class ZoneEventBullyDoggo : ZoneEvent
    {
        [JsonProperty]
        public int OwnerQuestUID = -1;

        [JsonProperty]
        public int UIDDoggo = -1;

        [JsonProperty]
        public int RoundsDramaTrigger = 2;

        [JsonProperty]
        public int RoundsEventTrigger = 4;

        [JsonProperty]
        public bool IntroDramaPlayed = false;

        internal virtual HashSet<string> DramaStartStepList => new HashSet<string>
        {
            //"doggohint_FG_1",
        };

        public ZoneEventBullyDoggo(QuestBullyDoggo ownerQuest)
        {
            OwnerQuestUID = ownerQuest.uid;
            UIDDoggo = ownerQuest.UIDDoggo;
        }

        public override void OnTickRound()
        {
            if (!IsExistQuest())
            {
                this.Kill();
                return;
            }

            // PCパーティー以外の攻撃や攻撃者不明などで死んだ場合はOnKillCharaが発火しないので、OnTickRoundで逐次存在確認をする必要がある
            if (!IsExistDoggo())
            {
                var ownerQuest = EClass.game.quests.list.Where(q => q is QuestBullyDoggo qb && qb.uid == OwnerQuestUID).FirstOrDefault();
                if (ownerQuest != null)
                {
                    ownerQuest.Complete();
                }

                this.Kill();
                return;
            }

            if (!IntroDramaPlayed)
            {
                RoundsDramaTrigger--;

                if (RoundsDramaTrigger < 0)
                {
                    PlayIntroDrama();
                }
                return;
            }

            RoundsEventTrigger--;
            if (RoundsEventTrigger < 0)
            {
                // 位置のヒントとして定期的に大橋純子に電話してもらう
                AwooDoggo();
                RoundsEventTrigger = 4;
            }
        }

        internal virtual void PlayIntroDrama()
        {
            var victim = CharaGen.CreateFromFilter("c_guest");

            var callbackAction = new Action(() =>
            {
                DramaWrapper.Release();
            });

            EClass.pc.pos.PlaySound("warcry");
            DramaWrapper.Lock();
            DramaWrapper.SetCallbackAction(callbackAction);
            DramaWrapper.PlayDrama(victim, DramaStartStepList.RandomItem());
            IntroDramaPlayed = true;
        }

        internal virtual void AwooDoggo()
        {
            var doggo = EClass._zone.map.charas.Where(c => c.uid == UIDDoggo).FirstOrDefault();

            if (doggo == null)
            {
                this.Kill();
                return;
            }

            int doggoDistance = EClass.pc.pos.Distance(doggo.pos);
            Vector3 vector = default(Vector3);
            vector.z = 0;

            switch (true)
            {
                case var _ when doggoDistance < 5:
                    Msg.Say("byakko_mod_doggo_hint_close");
                    break;
                case var _ when doggoDistance < 12:
                    Msg.Say("byakko_mod_doggo_hint_near");
                    EClass.Sound.Play("warcry", vector, 1f);
                    break;
                case var _ when doggoDistance < 20:
                    Msg.Say("byakko_mod_doggo_hint_distanced");
                    EClass.Sound.Play("warcry", vector, 0.8f);
                    break;
                case var _ when doggoDistance < 40:
                    Msg.Say("byakko_mod_doggo_hint_far");
                    EClass.Sound.Play("warcry", vector, 0.5f);
                    break;
                default:
                    Msg.Say("byakko_mod_doggo_hint_toofar");
                    EClass.Sound.Play("warcry", vector, 0.3f);
                    break;
            }
        }

        private bool IsExistQuest()
        {
            return EClass.game.quests.list.Any(q => q is QuestBullyDoggo qb && qb.UIDDoggo == this.UIDDoggo);
        }

        private bool IsExistDoggo()
        {
            return EClass._zone.map.charas.Any(c => c.uid == this.UIDDoggo);
        }
    }
}
