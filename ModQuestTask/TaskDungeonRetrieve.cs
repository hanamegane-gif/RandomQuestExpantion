using Newtonsoft.Json;
using RandomQuestExpantion.ModQuests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RandomQuestExpantion.ModQuestTask
{
    class TaskDungeonRetrieve : QuestTask
    {
        [JsonProperty]
        public bool hasNefiaBossKilled = false;

        public override bool IsComplete()
        {
            // OnKillCharaイベントハンドラに登録するためにTaskを使うが、ここで依頼終了判定を行わない
            return false;
        }

        public override void OnKillChara(Chara c)
        {
            if (c.IsPCFaction)
            {
                return;
            }

            if (hasNefiaBossKilled)
            {
                return;
            }

            if (IsNefiaBoss(c))
            {
                owner.bonusMoney += CalcBonusMoney(c);

                var questInstance = (QuestDungeonRetrieve)this.owner;
                var targetIdThing = questInstance.idThing;
                var spawnPosition = c.pos.GetNearestPoint(allowInstalled: false, minRadius: 1);
                var generateLv = c.LV;

                SpawnQuestChest(spawnPosition, targetIdThing, generateLv);
                hasNefiaBossKilled = true;
            }
        }

        // 追加で出る宝箱に入れておけば多分気付くはず！ということで配達対象を神秘箱に入れて生成
        internal void SpawnQuestChest(Point spawnPosition, string IdThing, int generateLv)
        {
            var questChest = GenerateQuestChest();
            var distribution = GenerateDistribution(IdThing, generateLv);
            questChest.AddThing(distribution);
            EClass._zone.AddCard(questChest, spawnPosition).Install();
        }

        internal Thing GenerateQuestChest()
        {
            var generated = ThingGen.Create("chest_treasure");
            // めんどくさくなるだけなので鍵はかけない
            generated.c_lockLv = 0;

            // 生成時に中身まで作られてしまうので中身は空にしておく
            generated.things.DestroyAll();
            return generated;
        }

        internal Thing GenerateDistribution(string IdThing, int generateLv)
        {
            // 奇跡以上確定の場合のバニラの神器率は5%
            // 20%は高すぎるか？
            var gearRarity = (EClass.rnd(5) == 0) ? Rarity.Mythical : Rarity.Legendary;
            CardBlueprint.SetRarity(gearRarity);

            var generatedGear = ThingGen.Create(IdThing, lv: generateLv);

            // 素材エンチャが破壊に巻き込まれないようにするため一旦ダークマターにする
            var originalMaterial = generatedGear.material;
            generatedGear.ChangeMaterial("void");

            for (int i = 0; i < 2; i++)
            {
                RemoveEnchantRandomOne(generatedGear);
            }

            for (int i = 0; i < 2; i++)
            {
                var bonusEnchant = PickBonusEnchant(generatedGear, generateLv);
                if (bonusEnchant == null)
                {
                    continue;
                }

                var bonusEnchantStrength = CalcEnchantStrength(bonusEnchant, generateLv);
                generatedGear.elements.ModBase(bonusEnchant.id, bonusEnchantStrength);
            }

            generatedGear.ChangeMaterial(originalMaterial);

            generatedGear.Identify(show: false, idtSource: IDTSource.SuperiorIdentify);
            generatedGear.isStolen = true;

            var questInstance = (QuestDungeonRetrieve)this.owner;
            questInstance.SetDistribution(generatedGear);


            return generatedGear;
        }

        internal SourceElement.Row PickBonusEnchant(in Thing generatedGear, int generateLv)
        {
            var gearCategory = (generatedGear.category.IsChildOf("melee") ? "melee" : "armor");

            SourceElement.Row bonusEnchant = null;
            if (gearCategory == "melee")
            {
                bonusEnchant = PickMeleeBonusEnchant(generatedGear, generateLv);
            }
            else
            {
                bonusEnchant = PickArmorBonusEnchant(generatedGear, generateLv);
            }

            return bonusEnchant;
        }

        internal SourceElement.Row PickArmorBonusEnchant(in Thing generatedGear, int generateLv)
        {
            var candidateList = new List<SourceElement.Row>();
            int sumChance = 0;
            var gearCategory = generatedGear.category;

            // chanceによる抽選は残しつつレアエンチャは出やすくする
            Func<SourceElement.Row, bool> rareEnchantFilter = row => true;
            int filterRoll = EClass.rnd(100);

            if (filterRoll < 12)
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
            else if (filterRoll < 24)
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
            else if (filterRoll < 36)
            {
                // 戦闘系
                rareEnchantFilter = row => row.categorySub == "combat";
            }
            else if (filterRoll < 48)
            {
                // 耐性
                rareEnchantFilter = row => row.type == "Resistance";
            }
            else if (filterRoll < 60)
            {
                // 生産系
                rareEnchantFilter = row => row.categorySub == "craft" || row.categorySub == "labor";
            }
            else if (filterRoll < 68)
            {
                // 慧眼反魔突撃者パリィ不屈
                rareEnchantFilter = row => row.category == "attribute" && 
                (
                    row.id == ENC.encHit ||
                    row.id == SKILL.antiMagic ||
                    row.id == ENC.rusher ||
                    row.id == ENC.parry || 
                    row.id == ENC.guts
                );
            }
            else if (filterRoll < 76)
            {
                // 魔法強化信仰見切り盾の暴君射撃防御
                rareEnchantFilter = row => row.category == "attribute" &&
                (
                    row.id == ENC.encSpell ||
                    row.id == SKILL.faith ||
                    row.id == SKILL.evasionPlus ||
                    row.id == ENC.basher ||
                    row.id == ENC.defense_range
                );
            }
            else if (filterRoll == 99)
            {
                // ルーンの器
                return EClass.sources.elements.rows.Where(r => r.alias == "slot_rune").FirstOrDefault();
            }

            // フラグ系エンチャがボーナスで付くのはかわいそうなので弾いておく
            foreach (var enchant in EClass.sources.elements.rows.Where(r => r.IsEncAppliable(gearCategory) && !r.tag.Contains("flag") && rareEnchantFilter(r)))
            {
                if (enchant.LV < generateLv + 15)
                {
                    candidateList.Add(enchant);
                    sumChance += enchant.chance;
                }
            }

            if (sumChance == 0)
            {
                return null;
            }

            int enchantRoll = EClass.rnd(sumChance);
            int temp = 0;
            foreach (var enchant in candidateList)
            {
                temp += enchant.chance;
                if (enchantRoll < temp)
                {
                    return enchant;
                }
            }

            return null;
        }

        internal SourceElement.Row PickMeleeBonusEnchant(in Thing generatedGear, int generateLv)
        {
            var candidateList = new List<SourceElement.Row>();
            int sumChance = 0;
            var gearCategory = generatedGear.category;

            // chanceによる抽選は残しつつレアエンチャは出やすくする
            Func<SourceElement.Row, bool> rareEnchantFilter = row => true;
            int filterRoll = EClass.rnd(100);

            if (filterRoll < 12)
            {
                // 武器エンチャ系
                rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub != "eleAttack";
            }
            else if(filterRoll < 24)
            {
                // 属性追加
                rareEnchantFilter = row => row.encSlot == "weapon" && row.categorySub == "eleAttack";
            }
            else if (filterRoll < 36)
            {
                // 特攻
                rareEnchantFilter = row => row.encSlot == "weapon" && row.alias.Contains("bane_");
            }
            else if (filterRoll < 48)
            {
                // 耐性
                rareEnchantFilter = row => row.type == "Resistance";
            }
            else if (filterRoll < 60)
            {
                // 生産系
                rareEnchantFilter = row => row.categorySub == "craft" || row.categorySub == "labor";
            }
            else if (filterRoll < 68)
            {
                // 連撃慧眼ヴォーパル突撃者パリィ不屈
                rareEnchantFilter = row => row.category == "attribute" &&
                (
                    row.id == ENC.mod_flurry ||
                    row.id == ENC.encHit ||
                    row.id == SKILL.vopal ||
                    row.id == ENC.rusher ||
                    row.id == ENC.guts
                );
            }
            else if (filterRoll < 76)
            {
                // 逆襲魔法強化全特攻盾の暴君射撃防御
                rareEnchantFilter = row => row.category == "attribute" &&
                (
                    row.id == ENC.mod_frustration ||
                    row.id == ENC.encSpell ||
                    row.id == ENC.bane_all ||
                    row.id == ENC.basher ||
                    row.id == ENC.defense_range
                );
            }
            else if (filterRoll == 99)
            {
                // ルーンの器
                return EClass.sources.elements.rows.Where(r => r.alias == "slot_rune").FirstOrDefault();
            }

            // フラグ系エンチャがボーナスで付くのはかわいそうなので弾いておく
            foreach (var enchant in EClass.sources.elements.rows.Where(r => r.IsEncAppliable(gearCategory) && !r.tag.Contains("flag") && rareEnchantFilter(r)))
            {
                if (enchant.LV < generateLv + 15)
                {
                    candidateList.Add(enchant);
                    sumChance += enchant.chance;
                }
            }

            if (sumChance == 0)
            {
                return null;
            }

            int roll = EClass.rnd(sumChance);
            int temp = 0;
            foreach (var enchant in candidateList)
            {
                temp += enchant.chance;
                if (roll < temp)
                {
                    return enchant;
                }
            }

            return null;
        }
        internal int CalcEnchantStrength(in SourceElement.Row enchant, int generateLv)
        {
            if (enchant.alias == "slot_rune")
            {
                return (EClass.rnd(4) == 0) ? 2 : 1;
            }

            int maxStrength = (int)((3 + Mathf.Min(generateLv / 10, 15)) + Mathf.Sqrt(generateLv * enchant.encFactor / 100));

            int strength = (maxStrength * 7 / 10) + EClass.rnd(1 + maxStrength * 3 / 10);
            strength = (enchant.mtp + strength) / enchant.mtp;

            if (enchant.encFactor == 0 && strength > 25)
            {
                strength = 25;
            }

            return strength;
        }

        internal int CalcBonusMoney(in Chara boss)
        {
            int baseMoney = Mathf.Clamp((3 + boss.LV) * 10, 40, 100000000);

            // curveは使うがバニラ依頼の強敵ボーナスはあまりにもしょっぱすぎるのでrate高め
            return EClass.curve(baseMoney, 500, 2000, 90);
        }

        internal void RemoveEnchantRandomOne(Thing generatedGear)
        {
            var relpaceTargetEnchant = generatedGear.elements.dict.Values.Where(e => e._source.category == "attribute" || e._source.category == "skill" || e._source.category == "enchant").RandomItem();

            if (relpaceTargetEnchant != null)
            {
                generatedGear.elements.SetBase(relpaceTargetEnchant.id, 0);
            }
        }

        internal bool IsNefiaBoss(in Chara killedChara)
        {
            // ボス討伐時はzone.Bossをnullにした後にOnKillCharaが呼ばれるため、EClass._zone.Boss == killedCharaで楽に判定できない
            // まともにネフィアの主を判定する方法がないので力業

            // 「ネフィアの主＝最下層にいるボス」として、最下層かどうかはShouldMakeExitで判定する
            if (!EClass._zone.IsNefia || EClass._zone.ShouldMakeExit)
            {
                return false;
            }

            // 争いの祠で出てくるのもボス扱いなので引っかからないようにする
            if (EClass._zone.Boss != null)
            {
                return false;
            }

            return killedChara.c_bossType == BossType.Boss;
        }
    }
}
