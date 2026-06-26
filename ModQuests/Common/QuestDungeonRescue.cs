using Newtonsoft.Json;
using RandomQuestExpantion.General;
using RandomQuestExpantion.ModNefia;
using RandomQuestExpantion.ModQuests.QuestAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RandomQuestExpantion.General.General;


namespace RandomQuestExpantion.ModQuests.Common
{
    public class QuestDungeonRescue : QuestRandom, IExtendDeadline
    {
        [JsonProperty]
        public int UIDBackZone;

        [JsonProperty]
        public int UIDQuestNefiaZone = -1;

        [JsonProperty]
        public int UIDHostageChara = -1;

        [JsonProperty]
        public string IdRewardThing = null;

        [JsonProperty]
        public bool HasRescued = false;

        public Zone GoBackZone => RefZone.Get(UIDBackZone);

        public Zone TargetNefiaZone => RefZone.Get(UIDQuestNefiaZone);

        public bool IsRescuing => UIDHostageChara == -1;

        public bool IsEscorting => UIDHostageChara != -1;

        public SourceThing.Row RewardSourceRow => (IdRewardThing != null) ? EClass.sources.things.map[IdRewardThing] : null;

        public Chara EscortTargetChara => (IsEscorting) ? EClass._map.FindChara(UIDHostageChara) : null;

        public override string RewardSuffix => "_byakko_mod_rescue";

        public override string RefDrama2 => RewardSourceRow.GetName();

        public override bool FameContent => true;

        public override int BaseMoney => source.money + EClass.curve(DangerLv, 20, 15) * 10;

        public virtual int DaysExtraDeadline => 1 + (7 - Mathf.Clamp(this.difficulty, 1, 7)) / 3;

        public override void OnInit()
        {
            // 報酬アイテムのIdを先に決める
            SetIdRewardThing();

        }

        public override void OnStart()
        {
            OnStartExtendDeadline();
            UIDBackZone = EClass._zone.uid;

            var questNefia = GenerateQuestNefia();
            if (questNefia == null)
            {
                // ネフィアが生成できない程何かしらで埋まっている場合は(実装のめんどくささとか諸々の都合で)しょうがないので完了扱いとする
                // ただし報酬ほぼなし
                Msg.Say("byakko_mod_rescue_exception");
                Complete();
            }
            else
            {
                Msg.Say("discoverZone", questNefia.NameWithDangerLevel);
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

            if (!HasRescued)
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
                string nefiaName = TargetNefiaZone?.Name ?? "???";
                return "byakko_mod_progress_dungeon_rescue".lang(nefiaName);
            }
            else
            {
                string hostageName = EscortTargetChara?.GetName(NameStyle.Full) ?? "???";
                string townName = GoBackZone?.Name ?? "???";
                return "byakko_mod_progress_dungeon_rescue_back".lang(hostageName, townName);
            }
        }

        public virtual void OnNefiaBeaten(in Chara boss)
        {
            if (boss.currentZone.GetTopZone().uid == UIDQuestNefiaZone)
            {
                var chara = SpawnEscortTarget();
                int charaLv = EClass.curve(boss.LV / 5 + 10, 120, 200, 90);
                chara.SetLv(charaLv);

                var callbackAction = new Action(() =>
                {
                    // Drama前にミニオンにするとDrama内の#1~#5が置換されないバグが起きる
                    chara.MakeMinion(EClass.pc);
                    UIDHostageChara = chara.uid;
                    chara.Talk("parasite", null, null, forceSync: true);
                    HasRescued = true;
                    DramaWrapper.Release();
                });

                DramaWrapper.Lock();
                DramaWrapper.SetCallbackAction(callbackAction);
                DramaWrapper.SetArgumentStrings(GoBackZone.Name);
                DramaWrapper.PlayDrama(chara, "rescue_common_thank1");
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
            Rand.SetSeed(this.uid);
            var gearRarity = (EClass.rnd(5) == 0) ? Rarity.Mythical : Rarity.Legendary;
            var bp = new CardBlueprint { rarity = gearRarity, blesstedState = BlessedState.Normal };
            CardBlueprint.Set(bp);

            var generatedGear = ThingGen.Create(IdRewardThing, lv: this.dangerLv * 3 / 2);

            // chanceによる抽選は残しつつレアエンチャは出やすくする
            Func<Func<SourceElement.Row, bool>> rareEnchantFilter;
            if (generatedGear.category.IsChildOf("melee"))
            {
                rareEnchantFilter = GetWeaponEnchantFilter;
            }
            else
            {
                rareEnchantFilter = GetArmorEnchantFilter;
            }

            AddBonusRareEnchants(generatedGear, 2 + EClass.rnd(2), this.dangerLv * 3 / 2, rareEnchantFilter);
            generatedGear.Identify(show: false, IDTSource.SuperiorIdentify);
            Rand.SetSeed();
            return generatedGear;
        }

        public virtual Zone GenerateQuestNefia()
        {
            var region = EClass.world.region;
            region.InitElomap();
            var centerPoint = new Point(GoBackZone.x, GoBackZone.y);
            bool isWaterNefia = region.elomap.IsWater(centerPoint.x, centerPoint.z);
            var spawnPoint = region.GetRandomPoint
            (
                centerPoint.x,
                centerPoint.z,
                radius: 8,
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
            var createdNefia = CreateRandomSite(region, spawnPoint, nefiaSourceId, dangerLv);
            (createdNefia as IQuestRandomNefia).RevertToVanillaZoneId();

            return createdNefia;
        }

        public virtual SourceZone.Row GetRandomQuestNefiaSource(in Point genPos)
        {
            bool isWater = EClass.world.region.elomap.IsWater(genPos.x, genPos.z);
            return EClass.sources.zones.rows.Where(r => r.tag.Contains("rqxrandomquest"))
                                            .Where(r => (isWater) ? r.idProfile == "DungeonWater" : r.idProfile != "DungeonWater")
                                            .ToList().RandomItemWeighted(r => r.chance);
        }

        public Quest OnStartExtendDeadline()
        {
            deadline += DaysExtraDeadline * Date.DayToken;
            return this;
        }

        public string GetAltTextDeadline()
        {
            return AltTextDeadline((Hours >= 0) ? Hours : 0, DaysExtraDeadline);
        }

        public virtual int CalcNefiaDangerLv()
        {
            int nefiaLv = this.dangerLv;
            if (EClass.player.CountKeyItem("license_adv") == 0 && nefiaLv > 50)
            {
                nefiaLv = 41 + EClass.rnd(10);
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

        internal virtual Func<SourceElement.Row, bool> GetArmorEnchantFilter()
        {
            Func<SourceElement.Row, bool> rareEnchantFilter = row => true;
            int filterRoll = EClass.rnd(100);
            int chanceSum = 0;

            if (filterRoll < (chanceSum += 12))
            {
                // 肉体系主能力
                rareEnchantFilter = row => row.category == "attribute" &&
                (
                    row.id == SKILL.STR ||
                    row.id == SKILL.END ||
                    row.id == SKILL.DEX ||
                    row.id == SKILL.CHA
                );
            }
            else if (filterRoll < (chanceSum += 12))
            {
                // 精神系主能力
                rareEnchantFilter = row => row.category == "attribute" &&
                (
                    row.id == SKILL.PER ||
                    row.id == SKILL.LER ||
                    row.id == SKILL.WIL ||
                    row.id == SKILL.MAG
                );
            }
            else if (filterRoll < (chanceSum += 12))
            {
                // 戦闘系
                rareEnchantFilter = row => row.categorySub == "combat";
            }
            else if (filterRoll < (chanceSum += 12))
            {
                // 耐性
                rareEnchantFilter = row => row.type == "Resistance";
            }
            else if (filterRoll < (chanceSum += 12))
            {
                // 生産系
                rareEnchantFilter = row => row.categorySub == "craft" || row.categorySub == "labor";
            }
            else if (filterRoll < (chanceSum += 8))
            {
                // 慧眼反魔突撃者パリィ不屈
                rareEnchantFilter = row =>
                (
                    row.id == ENC.encHit ||
                    row.id == SKILL.antiMagic ||
                    row.id == ENC.rusher ||
                    row.id == ENC.parry ||
                    row.id == ENC.guts
                );
            }
            else if (filterRoll < (chanceSum += 8))
            {
                // 魔法強化信仰見切り盾の暴君射撃防御
                rareEnchantFilter = row =>
                (
                    row.id == ENC.encSpell ||
                    row.id == SKILL.faith ||
                    row.id == SKILL.evasionPlus ||
                    row.id == ENC.basher ||
                    row.id == ENC.defense_range
                );
            }

            return rareEnchantFilter;
        }

        internal virtual Func<SourceElement.Row, bool> GetWeaponEnchantFilter()
        {
            Func<SourceElement.Row, bool> rareEnchantFilter = row => true;
            int filterRoll = EClass.rnd(100);
            int chanceSum = 0;

            if (filterRoll < (chanceSum += 12))
            {
                // 武器エンチャ系
                rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub != "eleAttack";
            }
            else if (filterRoll < (chanceSum += 12))
            {
                // 属性追加
                rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub == "eleAttack";
            }
            else if (filterRoll < (chanceSum += 12))
            {
                // 特攻
                rareEnchantFilter = row => row.encSlot == "weapon" && row.alias.Contains("bane_");
            }
            else if (filterRoll < (chanceSum += 12))
            {
                // 耐性
                rareEnchantFilter = row => row.type == "Resistance";
            }
            else if (filterRoll < (chanceSum += 12))
            {
                // 生産系
                rareEnchantFilter = row => row.categorySub == "craft" || row.categorySub == "labor";
            }
            else if (filterRoll < (chanceSum += 8))
            {
                // 連撃慧眼ヴォーパル突撃者パリィ不屈
                rareEnchantFilter = row =>
                (
                    row.id == ENC.mod_flurry ||
                    row.id == ENC.encHit ||
                    row.id == SKILL.vopal ||
                    row.id == ENC.rusher ||
                    row.id == ENC.guts
                );
            }
            else if (filterRoll < (chanceSum += 8))
            {
                // 逆襲魔法強化全特攻盾の暴君射撃防御
                rareEnchantFilter = row =>
                (
                    row.id == ENC.mod_frustration ||
                    row.id == ENC.encSpell ||
                    row.id == ENC.bane_all ||
                    row.id == ENC.basher ||
                    row.id == ENC.defense_range
                );
            }

            return rareEnchantFilter;
        }
    }
}
