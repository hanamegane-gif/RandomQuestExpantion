using Newtonsoft.Json;
using RandomQuestExpantion.DayBreak;
using RandomQuestExpantion.ModQuests.Common;
using System;
using UnityEngine;
using static RandomQuestExpantion.General.General;

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

        public virtual void OnNefiaBeaten(Chara boss)
        {
            if (hasNefiaBossKilled)
            {
                return;
            }

            owner.SetBonusMoney(owner.GetBonusMoney() + CalcBonusMoney(boss));

            var questInstance = this.owner as QuestDungeonRetrieve;
            string targetIdThing = questInstance.idThing;
            var spawnPosition = boss.pos.GetNearestPoint(allowInstalled: false, minRadius: 1);
            int generateLv = boss.LV;

            SpawnQuestChest(spawnPosition, targetIdThing, generateLv);
            hasNefiaBossKilled = true;
        }

        internal virtual int CalcBonusMoney(in Chara boss)
        {
            int baseMoney = Mathf.Clamp((3 + boss.LV) * 10, 40, 20000000);

            // curveは使うがバニラ依頼の強敵ボーナスはあまりにもしょっぱすぎるのでrate高め
            return EClass.curve(baseMoney, 500, 2000, 90);
        }

        // 追加で出る宝箱に入れておけば多分気付くはず！ということで配達対象を神秘箱に入れて生成
        internal virtual void SpawnQuestChest(Point spawnPosition, string IdThing, int generateLv)
        {
            var questChest = GenerateQuestChest();
            var distribution = GenerateDistribution(IdThing, generateLv);
            questChest.AddThing(distribution);
            EClass._zone.AddCard(questChest, spawnPosition).Install();
        }

        internal virtual Thing GenerateQuestChest()
        {
            var generated = ThingGen.Create("chest_treasure");
            // めんどくさくなるだけなので鍵はかけない
            generated.c_lockLv = 0;

            // 生成時に中身まで作られてしまうので中身は空にしておく
            generated.things.DestroyAll();
            return generated;
        }

        internal virtual Thing GenerateDistribution(string IdThing, int generateLv)
        {
            // 奇跡以上確定の場合のバニラの神器率は5%
            // 20%は高すぎるか？
            var gearRarity = (EClass.rnd(5) == 0) ? Rarity.Mythical : Rarity.Legendary;
            var bp = new CardBlueprint { rarity = gearRarity, blesstedState = BlessedState.Normal };
            CardBlueprint.Set(bp);

            var generatedGear = ThingGen.Create(IdThing, lv: generateLv);

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

            generatedGear = AddBonusRareEnchants(generatedGear, enchantNum: 2, generateLv, rareEnchantFilter);

            generatedGear.Identify(show: false, idtSource: IDTSource.SuperiorIdentify);
            generatedGear.isStolen = true;

            var questInstance = this.owner as QuestDungeonRetrieve;
            questInstance.SetDistribution(generatedGear);


            return generatedGear;
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
