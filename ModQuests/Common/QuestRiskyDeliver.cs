using Newtonsoft.Json;
using RandomQuestExpantion.Config;
using RandomQuestExpantion.ModQuests.QuestAttribute;
using RandomQuestExpantion.ModZonePreenter;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RandomQuestExpantion.General.General;

namespace RandomQuestExpantion.ModQuests.Common
{
    public class QuestRiskyDeliver : QuestDeliver, IOverrideWildernessEncounter, IExtendDeadline
    {
        [JsonProperty]
        public int TravelEncounterdCount = 0;

        public override bool ForbidTeleport => false;

        public override bool FameContent => true;

        public int KOROSHITEDEMOUBAITORUChance => 10;

        [JsonProperty]
        public Thing Distribution = null;

        public bool CanDeliver => Distribution != null;

        // 初回:90% 2回目: 40% 3回目: 15%
        public int EncounterChance => (100 / (int)(Math.Pow(2, TravelEncounterdCount))) - 10;

        public virtual int DaysExtraDeadline => 2 + (7 - Mathf.Clamp(this.difficulty, 1, 7)) / 2;

        public override int BaseMoney => (int)Math.Min((long)source.money + (long)EClass.curve(DangerLv, 500, 2000, 90) * 2L, Int32.MaxValue / 200);

        public override void OnStart()
        {
            // 依頼受注時に武器種を決定する
            // 特定武器種を狙い撃ちしてリロードができるのは強すぎる(乱数調整Modが流行っているとはいえ)
            SetIdThing();

            var thing = GenerateDistribution();
            Distribution = thing;
            Msg.Say("get_quest_item");
            EClass.pc.Pick(thing);
            OnStartExtendDeadline();
        }

        public override void OnComplete()
        {
            base.OnComplete();
            if (EClass.rnd(100) >= KOROSHITEDEMOUBAITORUChance)
            {
                Distribution.Destroy();
            }
        }

        public override void SetIdThing()
        {
            CardRow cardRow = null;
            while (cardRow == null)
            {
                var r = PickTargetGearCategory();
                cardRow = SpawnListThing.Get("cat_" + r.id, (SourceThing.Row s) => EClass.sources.categories.map[s.category].IsChildOf(r.id)).Select();
            }
            idThing = cardRow.id;
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

        public override string GetTextProgress()
        {
            string isInInvText = ((GetDestThing() != null) ? "supplyInInv".lang().TagColor(FontColor.Good) : "supplyNotInInv".lang());

            string @ref = (base.DestZone.dictCitizen.TryGetValue(uidTarget) ?? "???") + " (" + base.DestZone.Name + ")";
            return "progressDeliver".lang(this.Distribution.GetName(NameStyle.Full), @ref, isInInvText);
        }

        public virtual void OnWildernessEncounted(Zone newZone)
        {
            TravelEncounterdCount++;
            newZone.events.AddPreEnter(new ZonePreEnterDeliverBrigand(this));
        }

        public virtual bool ShouldOverrideEncounter()
        {
            return ListDestThing(onlyFirst: true).Any() && (EClass.rnd(100) < EncounterChance);
        }

        public virtual int GetOverlappingPriority()
        {
            return 4 - TravelEncounterdCount;
        }

        internal virtual Thing GenerateDistribution()
        {
            Rand.SetSeed(this.uid);

            var gearRarity = (EClass.rnd(5) == 0) ? Rarity.Mythical : Rarity.Legendary;
            var bp = new CardBlueprint { rarity = gearRarity, blesstedState = BlessedState.Normal, lv = this.dangerLv };
            CardBlueprint.Set(bp);

            var generatedGear = ThingGen.Create(idThing, lv: this.dangerLv * 3 / 2);
            generatedGear = AddBonusRareEnchants(generatedGear, enchantNum: 2, dangerLv, GetWeaponEnchantFilter);

            generatedGear.Identify(show: false, idtSource: IDTSource.SuperiorIdentify);

            Rand.SetSeed();
            return generatedGear;
        }

        public override int GetRewardPlat(int money)
        {
            int bonusPlat = (ModConfig.RewardPlatRate > 0) ? EClass.curve(DangerLv / 25, 10, 20, 80) * ModConfig.RewardPlatRate / 100 : EClass.rndHalf((int)Mathf.Sqrt(money / 200));
            return 1 + EClass.rnd(2) + bonusPlat;
        }

        private SourceCategory.Row PickTargetGearCategory()
        {
            var gearCategories = new List<SourceCategory.Row>();

            // 武器のみ
            foreach (var row in EClass.sources.categories.rows)
            {
                if (row.deliver > 0 && row.IsChildOf("melee"))
                {
                    gearCategories.Add(row);
                }
            }

            return gearCategories.RandomItem();
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
