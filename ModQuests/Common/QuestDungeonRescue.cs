using B83.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace RandomQuestExpantion.ModQuests.Common
{
    class QuestDungeonRescue : QuestRandom
    {
        [JsonProperty]
        public int UIDQuestNefiaZone = -1;

        [JsonProperty]
        public int UIDHostageChara = -1;

        [JsonProperty]
        public string IdRewardThing = null;

        public Zone DestNefiaZone => chara.homeZone;

        public Zone GoBackZone => chara.homeZone;

        public bool IsRescuing => UIDHostageChara == -1;

        public bool IsEscorting => UIDHostageChara != -1;

        public SourceThing.Row RewardSourceRow => (IdRewardThing != null) ? EClass.sources.things.map[IdRewardThing] : null;

        public virtual Chara EscortTargetChara => (IsEscorting) ? EClass._map.FindChara(UIDHostageChara) : null;

        public override string RefDrama1 => RewardSourceRow.GetName();

        public override bool FameContent => true;

        public override void OnInit()
        {
            // 報酬アイテムのIdを先に決める
            SetIdRewardThing();
        }

        public override void OnStart()
        {
            var questNefia = GenerateQuestNefia();

            if (questNefia == null)
            {
                // ネフィアが生成できない程何かしらで埋まっている場合は(実装のめんどくささとか諸々の都合で)しょうがないので完了扱いとする
                // ただし報酬ほぼなし
                Complete();
            }
            else
            {
                UIDQuestNefiaZone = questNefia.uid;
            }
        }

        public override void OnEnterZone()
        {
            if (!IsEscorting)
            {
                return;
            }

            if (EscortTargetChara == null || EscortTargetChara.isDead)
            {
                Fail();
            }
            else if (EClass._zone == GoBackZone)
            {
                Complete();
                EscortTargetChara.Talk("thanks", null, null, forceSync: true);
                ReleaseEscort();
            }
        }

        public override void OnFail()
        {
            ReleaseEscort();
        }

        public override void OnDropReward()
        {
            base.OnDropReward();

            if (!IsRescuing)
            {
                // 救助中の依頼完了はネフィア生成に失敗したと見なす
                return;
            }

            var reward = GenerateQuestReward();
            DropReward(reward);
        }

        public override string GetTextProgress()
        {
            if (IsRescuing)
            {
                return "ネフィアから救出する";
            }
            else
            {
                return GoBackZone.Name + "まで送り届ける";
            }
        }

        public virtual void OnNefiaBeaten(in Chara boss)
        {
            if (boss.currentZone.GetTopZone().uid == UIDQuestNefiaZone)
            {
                var chara = SpawnEscortTarget();
                chara.MakeMinion(EClass.pc);
                UIDHostageChara = chara.uid;
                chara.Talk("parasite", null, null, forceSync: true);
            }
        }

        public virtual void OnNefiaRetreated(in Zone currentZone)
        {
            if (IsRescuing && (currentZone.GetTopZone().uid == UIDQuestNefiaZone))
            {
                Fail();
            }
        }

        public virtual void SetIdRewardThing()
        {
            CardRow cardRow = null;
            while (cardRow == null)
            {
                var r = PickTargetGearCategory();
                cardRow = SpawnListThing.Get("cat_" + r.id, (SourceThing.Row s) => EClass.sources.categories.map[s.category].IsChildOf(r.id)).Select();
            }
            IdRewardThing = cardRow.id;
        }

        public virtual SourceCategory.Row PickTargetGearCategory()
        {
            var gearCategories = new List<SourceCategory.Row>();

            foreach (var row in EClass.sources.categories.rows)
            {
                if (row.deliver > 0 && (row.IsChildOf("melee") || row.IsChildOf("armor")) && row.id != "lightsource")
                {
                    gearCategories.Add(row);
                }
            }

            return gearCategories.RandomItem();
        }

        public virtual Chara SpawnEscortTarget()
        {
            var chara = CharaGen.CreateFromFilter("c_neutral", 10);
            chara.SetLv(EClass._zone.DangerLv);
            EClass._zone.AddCard(chara, EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false));
            return chara;
        }

        public virtual Thing GenerateQuestReward()
        {
            var thing = ThingGen.Create("MOD_byakko_RQX_guilpo_merchant");
            thing.Identify(show: false, IDTSource.SuperiorIdentify);
            return thing;
        }

        public virtual Zone GenerateQuestNefia()
        {
            var region = EClass.world.region;
            region.InitElomap();
            var centerPoint = new Point(GoBackZone.x, GoBackZone.y);
            bool isWaterNefia = region.elomap.IsWater(centerPoint.x, centerPoint.z);
            var spawnPoint = region.GetRandomPoint(
                                        centerPoint.x, centerPoint.z, 8,
                                        increaseRadius: false,
                                        type: (isWaterNefia ? ElomapSiteType.NefiaWater : ElomapSiteType.Nefia)
                                    );

            // 道や既存ネフィアで埋まっている場合は取得できない
            if (spawnPoint == null)
            {
                return null;
            }

            var nefiaSourceId = GetRandomQuestNefiaSource(spawnPoint).id;
            int dangerLv = CalcNefiaDangerLv();
            return CreateRandomSite(region, spawnPoint, nefiaSourceId, dangerLv);
        }

        public virtual SourceZone.Row GetRandomQuestNefiaSource(in Point genPos)
        {
            bool isWater = EClass.world.region.elomap.IsWater(genPos.x, genPos.z);
            return EClass.sources.zones.rows.Where(r => r.tag.Contains("rqxrandomquest"))
                                            .Where(r => (isWater) ? r.idProfile == "DungeonWater" : r.idProfile != "DungeonWater")
                                            .ToList().RandomItemWeighted(r => r.chance);
        }

        public virtual int CalcNefiaDangerLv()
        {
            int dangerLv = lv;
            if (EClass.player.CountKeyItem("license_adv") == 0 && dangerLv > 50)
            {
                dangerLv = 41 + EClass.rnd(10);
            }

            return dangerLv;
        }

        private void ReleaseEscort()
        {
            if (EscortTargetChara != null)
            {
                EscortTargetChara.UnmakeMinion();
                if (EClass._zone.IsRegion)
                {
                    EscortTargetChara.Destroy();
                }
                else
                {
                    EscortTargetChara.SetSummon(60);
                }
            }
        }

        private Zone CreateRandomSite(Region region, Point pos, string idSource, int dangerLv)
        {
            Zone zone = SpatialGen.Create(idSource, region, register: true, pos.x, pos.z) as Zone;
            zone._dangerLv = Mathf.Max(1, dangerLv);
            zone.isRandomSite = true;            
            zone.dateExpire = this.deadline + 2880; // 期限+2日
            region.elomap.SetZone(zone.x, zone.y, zone);
            return zone;
        }
    }
}
