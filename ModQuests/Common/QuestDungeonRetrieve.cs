using Newtonsoft.Json;
using RandomQuestExpantion.Config;
using RandomQuestExpantion.ModQuestTask;
using RandomQuestExpantion.Patch;
using System.Collections.Generic;
using UnityEngine;

namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestDungeonRetrieve : QuestSupply
    {
        public virtual bool UseWeight => true;

        public override bool ForbidTeleport => false;

        public override string RewardSuffix => "Hunt";

        public override string TextExtra2 => "noDeadLine".lang();

        // きっと気付かれるだろうなと思いつつころうばを仕込んでおく
        public int KOROSHITEDEMOUBAITORUChance => 10;

        [JsonProperty]
        public Thing Distribution = null;

        public bool CanDeliver => Distribution != null;

        public override void OnInit()
        {
            SetTask(new TaskDungeonRetrieve());
            num = GetDestNum();
            SetIdThing();
        }

        public override void OnStart()
        {
            deadline = 0;
            num = 1;
            base.OnStart();
        }

        public override void OnComplete()
        {
            base.OnComplete();
            if (EClass.rnd(100) >= KOROSHITEDEMOUBAITORUChance)
            {
                Distribution.Destroy();
            }
        }

        public virtual void OnNefiaBeaten(in Chara boss)
        {
            if (task != null)
            {
                task.OnNefiaBeatenMod(boss);
            }
        }

        // 配達対象アイテムのIdだけを決める、エンチャントを撃破ボスLvに依存させたいので実体はボス討伐時に作る
        public override void SetIdThing()
        {
            CardRow cardRow;
            do
            {
                SourceCategory.Row r = PickTargetGearCategory();
                cardRow = SpawnListThing.Get("cat_" + r.id, (SourceThing.Row s) => EClass.sources.categories.map[s.category].IsChildOf(r.id)).Select();
            }
            while (cardRow == null);
            idThing = cardRow.id;
        }

        public override int GetRewardPlat(int money)
        {
            int bonusPlat = (ModConfig.RewardPlatRate > 0) ? EClass.curve(bonusMoney / 400, 6, 10, 75) * ModConfig.RewardPlatRate / 100 : EClass.rndHalf((int)Mathf.Sqrt(money / 200));
            return 1 + EClass.rnd(2) + bonusPlat;
        }

        // 実際の配達対象を決めるのはこっち
        public void SetDistribution(in Thing _distribution)
        {
            Distribution = _distribution;
        }

        private SourceCategory.Row PickTargetGearCategory()
        {
            List<SourceCategory.Row> gearCategories = new List<SourceCategory.Row>();

            // 矢弾などをはじくため配達可能なカテゴリのみとする
            // 光源は面白そうだが出さない
            // 遠隔武器はエンチャをつけづらいしどうせmodだけ抜かれることになるので出さない
            foreach (SourceCategory.Row row in EClass.sources.categories.rows)
            {
                if (row.deliver > 0 && (row.IsChildOf("melee") || row.IsChildOf("armor")) && row.id != "lightsource")
                {
                    gearCategories.Add(row);
                }
            }

            return gearCategories.RandomItem();
        }

        public override string GetTextProgress()
        {
            string text = ((GetDestThing() != null) ? "supplyInInv".lang().TagColor(FontColor.Good) : "supplyNotInInv".lang());
            if (CanDeliver)
            {
                return "progressSupply".lang(Distribution.GetName(NameStyle.Full), text);
            }
            else
            {
                return "byakko_mod_progress_dungeon_retrieve".lang(sourceThing.GetName(), text);
            }
        }

        public override bool IsDestThing(Thing t)
        {
            if (Distribution == null)
            {
                return false;
            }

            if (t.parentCard != null && !t.parentCard.trait.CanUseContent)
            {
                return false;
            }

            if (t.uid != Distribution.uid)
            {
                return false;
            }

            if (!t.c_isImportant && (!t.IsContainer || t.things.Count == 0) && !t.isEquipped)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
