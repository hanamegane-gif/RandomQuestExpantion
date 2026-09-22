using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using UnityEngine;
using static BiomeProfile;

namespace RandomQuestExpantion.ModQuestEvent
{
    class ZoneEventDuel : ZoneEventSubdue
    {
        # region 冒険者の行動パターン
        private static Dictionary<string, Type> VanguardSpecialtyTypeResolver => new Dictionary<string, Type>
        {
            { "Assault", typeof(Assault) },
            { "Tank", typeof(Tank) },
            { "SpellBrade", typeof(SpellBrade) },
        };

        private static Dictionary<string, Type> RearguardTypeResolver => new Dictionary<string, Type>
        {
            { "Shooter", typeof(Shooter) },
            { "Caster", typeof(Caster) },
            { "Summoner", typeof(Summoner) },
        };

        private static Dictionary<string, Type> SupporterTypeResolver => new Dictionary<string, Type>
        {
            { "Enchanter", typeof(Enchanter) },
            { "Healer", typeof(Healer) },
            { "Enfeebler", typeof(Enfeebler) },
        };

        internal class Specialty
        {
            internal virtual void Apply(Chara c)
            {
                ApplyJob(c);
                ApplyCommonWeakness(c);
                ApplyStrength(c);
                ApplyWeakness(c);
                ApplyRandomAbility(c);
            }

            // バランス調整用の共通の弱点
            private void ApplyCommonWeakness(Chara c)
            {
                c.elements.ModBase(SKILL.dmgDealt, -30);
                c.elements.ModBase(SKILL.resDamage, -30);
            }

            internal virtual void ApplyJob(Chara c) { }

            internal virtual void ApplyStrength(Chara c) { }

            internal virtual void ApplyWeakness(Chara c) { }

            internal virtual void ApplyRandomAbility(Chara c) { }
        }

        private class Assault : Specialty
        {
            internal override void ApplyStrength(Chara c)
            {
                c.elements.ModBase(SKILL.PDR, 25);
            }
            internal override void ApplyWeakness(Chara c)
            {
                c.elements.ModBase(SKILL.EDR, -30);
            }

            internal override void ApplyJob(Chara c)
            {
                c.ChangeJob((EClass.rnd(3) == 0 ? "warrior" : EClass.rnd(2) == 0 ? "predator" : "farmer"));
            }

            internal override void ApplyRandomAbility(Chara c)
            {
                c.ability.Add(ABILITY.ActRush, 50, false);

                if (EClass.rnd(2) == 0)
                {
                    c.ability.Add(ABILITY.ActEntangle, 50, false);
                }
                else
                {
                    c.ability.Add(ABILITY.ActDraw, 50, false);
                }

                if (EClass.rnd(3) == 0)
                {
                    c.ability.Add(ABILITY.ActInsult, 50, false);
                }
                else if (EClass.rnd(2) == 0)
                {
                    c.ability.Add(ABILITY.ActWhirlwind, 50, false);
                }
                else
                {
                    c.ability.Add(ABILITY.ActBladeStorm2, 50, false);
                }
            }
        }

        private class Tank : Specialty
        {
            internal override void ApplyStrength(Chara c)
            {
                c.elements.ModBase(SKILL.resDamage, 34);
                c.elements.ModBase(SKILL.FPV, 10);
            }
            internal override void ApplyWeakness(Chara c)
            {
                c.elements.ModBase(SKILL.dmgDealt, -20);
            }

            internal override void ApplyJob(Chara c)
            {
                c.ChangeJob((EClass.rnd(3) == 0 ? "paladin" : EClass.rnd(2) == 0 ? "inquisitor" : "executioner"));
            }

            internal override void ApplyRandomAbility(Chara c)
            {
                if (EClass.rnd(2) == 0)
                {
                    c.SetFeat(FEAT.featMeatCushion, 1);
                }
                else
                {
                    c.SetFeat(FEAT.featLoyal, 1);
                }

                if (EClass.rnd(3) == 0)
                {
                    c.ability.Add(SPELL.SpHealHeavy, 50, false);
                }
                else if (EClass.rnd(2) == 0)
                {
                    c.ability.Add(ABILITY.ActDraw, 50, false);
                }
                else
                {
                    c.ability.Add(ABILITY.ActEntangle, 50, false);
                }
            }
        }

        private class SpellBrade : Specialty
        {
            internal override void ApplyStrength(Chara c)
            {
                c.elements.ModBase(SKILL.evasionPerfect, 20);
                c.elements.ModBase(SKILL.EDR, 20);
            }
            internal override void ApplyWeakness(Chara c)
            {
                c.elements.ModBase(SKILL.life, -20);
            }

            internal override void ApplyJob(Chara c)
            {
                c.ChangeJob((EClass.rnd(3) == 0 ? "thief" : EClass.rnd(2) == 0 ? "warmage" : "swordsage"));
            }

            internal override void ApplyRandomAbility(Chara c)
            {
                var elementId = EClass.sources.elements.rows.Where(r => r.categorySub == "eleAttack" && r.chance > 0).RandomItem().id - 910;

                // 手魔法
                c.ability.Add(50400 + elementId, 50, false);
                // 唄魔法
                c.ability.Add(50800 + elementId, 50, false);

                if (EClass.rnd(3) == 0)
                {
                    // 剣魔法
                    c.ability.Add(51000 + elementId, 50, false);
                }
                else if (EClass.rnd(2) == 0)
                {
                    c.ability.Add(SPELL.SpSpeedUp, 50, false);
                }
                else
                {
                    c.ability.Add(SPELL.SpDarkness, 50, false);
                }
            }
        }

        private class Enchanter : Specialty
        {
            internal override void ApplyStrength(Chara c)
            {
                c.elements.ModBase(SKILL.EDR, 25);

            }
            internal override void ApplyWeakness(Chara c)
            {
                c.elements.ModBase(SKILL.PDR, -30);
            }

            internal override void ApplyJob(Chara c)
            {
                c.ChangeJob((EClass.rnd(3) == 0 ? "pianist" : EClass.rnd(2) == 0 ? "bard" : "alchemist"));
            }

            internal override void ApplyRandomAbility(Chara c)
            {
                if (EClass.rnd(3) == 0)
                {
                    c.ability.Add(SPELL.SpHero, 50, true);
                }
                else if (EClass.rnd(2) == 0)
                {
                    c.ability.Add(SPELL.SpWisdom, 50, true);
                }
                else
                {
                    c.ability.Add(SPELL.SpCatsEye, 50, true);
                }

                if (EClass.rnd(3) == 0)
                {
                    c.ability.Add(SPELL.SpHolyShield, 50, true);
                }
                else if (EClass.rnd(2) == 0)
                {
                    c.ability.Add(SPELL.SpResEle, 50, true);
                }
                else
                {
                    c.ability.Add(SPELL.SpHolyVeil, 50, true);
                }

                if (EClass.rnd(3) == 0)
                {
                    c.ability.Add(SPELL.SpLevitate, 50, true);
                }
                else if (EClass.rnd(2) == 0)
                {
                    c.ability.Add(SPELL.SpSeeInvisible, 50, true);
                }
                else
                {
                    c.ability.Add(SPELL.SpDarkness, 50, false);
                }

                if (EClass.rnd(10) == 0)
                {
                    c.ability.Add(SPELL.SpSpeedUp, 50, false);
                }
            }
        }

        private class Healer : Specialty
        {
            internal override void ApplyStrength(Chara c)
            {
                c.elements.ModBase(SKILL.life, 20);
            }
            internal override void ApplyWeakness(Chara c)
            {
                c.elements.ModBase(SKILL.evasionPerfect, -25);
            }

            internal override void ApplyJob(Chara c)
            {
                c.ChangeJob((EClass.rnd(3) == 0 ? "priest" : EClass.rnd(2) == 0 ? "paladin" : "tourist"));
            }

            internal override void ApplyRandomAbility(Chara c)
            {
                if (EClass.rnd(3) == 0)
                {
                    c.ability.Add(SPELL.SpHealHeavy, 50, true);
                }
                else if (EClass.rnd(2) == 0)
                {
                    c.ability.Add(SPELL.SpHealCritical, 50, true);
                }
                else
                {
                    c.ability.Add(SPELL.SpHeal, 50, true);
                }

                if (EClass.rnd(3) == 0)
                {
                    c.ability.Add(SPELL.SpHOT, 50, true);
                }
                else if (EClass.rnd(2) == 0)
                {
                    c.ability.Add(SPELL.SpRemoveHex, 50, true);
                }
                else if (EClass.rnd(10) == 0)
                {
                    c.ability.Add(SPELL.SpRevive, 50, false);
                }
            }
        }

        private class Shooter : Specialty
        {
            internal override void ApplyStrength(Chara c)
            {
                c.elements.ModBase(SKILL.penetration, 20);

                // 精密射撃
                c.elements.ModBase(605, 20);
            }
            internal override void ApplyWeakness(Chara c)
            {
                c.elements.ModBase(SKILL.EDR, -20);
            }

            internal override void ApplyJob(Chara c)
            {
                c.ChangeJob((EClass.rnd(3) == 0 ? "archer" : EClass.rnd(2) == 0 ? "gunner" : "inquisitor"));
            }

            internal override void ApplyRandomAbility(Chara c)
            {
                if (EClass.rnd(3) == 0)
                {
                    c.SetFeat(FEAT.featRapidArrow, 1);
                }
                else if (EClass.rnd(2) == 0)
                {
                    c.SetFeat(FEAT.featCentaur, 1);
                }
                else
                {
                    // ノックバック+追尾
                    // アビリティじゃないけど
                    c.elements.ModBase(603, 20);
                    c.elements.ModBase(620, 20);
                }
            }
        }

        private class Caster : Specialty
        {
            internal override void ApplyStrength(Chara c)
            {
                c.elements.ModBase(SKILL.EDR, 40);
            }
            internal override void ApplyWeakness(Chara c)
            {
                c.elements.ModBase(SKILL.resDamage, -30);
            }

            internal override void ApplyJob(Chara c)
            {
                c.ChangeJob((EClass.rnd(3) == 0 ? "wizard" : EClass.rnd(2) == 0 ? "warmage" : "witch"));
            }

            internal override void ApplyRandomAbility(Chara c)
            {
                var elementId = EClass.sources.elements.rows.Where(r => r.categorySub == "eleAttack" && r.chance > 0).RandomItem().id - 910;

                if (EClass.rnd(2) == 0)
                {
                    // 矢魔法
                    c.ability.Add(50500 + elementId, 50, false);
                }
                else
                {
                    // 光線魔法
                    c.ability.Add(50300 + elementId, 50, false);
                }

                if (EClass.rnd(2) == 0)
                {
                    // 球魔法
                    c.ability.Add(50100 + elementId, 50, false);
                }
                else if (EClass.rnd(2) == 0)
                {
                    // フレア魔法
                    c.ability.Add(51200 + elementId, 50, false);
                }
            }
        }

        private class Enfeebler : Specialty
        {
            internal override void ApplyStrength(Chara c)
            {
                // なし
            }
            internal override void ApplyWeakness(Chara c)
            {
                c.elements.ModBase(SKILL.resDamage, -30);
            }

            internal override void ApplyJob(Chara c)
            {
                c.ChangeJob((EClass.rnd(3) == 0 ? "wizard" : EClass.rnd(2) == 0 ? "thief" : "witch"));
            }

            internal override void ApplyRandomAbility(Chara c)
            {
                if (EClass.rnd(2) == 0)
                {
                    c.ability.Add(SPELL.SpWeakness, 50, EClass.rnd(3) == 0);
                }
                else
                {
                    c.ability.Add(SPELL.SpWeakResEle, 50, EClass.rnd(3) == 0);
                }

                if (EClass.rnd(3) == 0)
                {
                    c.ability.Add(SPELL.SpSilence, 50, EClass.rnd(10) == 0);
                }
                else if (EClass.rnd(2) == 0)
                {
                    c.ability.Add(SPELL.SpNightmare, 50, EClass.rnd(7) == 0);
                }
                else
                {
                    c.ability.Add(SPELL.SpBane, 50, EClass.rnd(5) == 0);
                }

                if (EClass.rnd(10) == 0)
                {
                    c.ability.Add(ABILITY.ActCrySad, 50, false);
                }
                else
                {
                    c.ability.Add(ABILITY.ActGazeMana, 50, false);
                }

                if (EClass.rnd(10) == 0)
                {
                    c.ability.Add(SPELL.SpSpeedDown, 50, EClass.rnd(5) == 0);
                }
                else
                {
                    c.ability.Add(SPELL.SpBerserk, 50, false);
                }
            }
        }

        private class Summoner : Specialty
        {
            internal override void ApplyStrength(Chara c)
            {
                // なし
            }
            internal override void ApplyWeakness(Chara c)
            {
                c.elements.ModBase(SKILL.dmgDealt, -30);
                c.elements.ModBase(SKILL.resDamage, -30);
            }

            internal override void ApplyJob(Chara c)
            {
                c.ChangeJob((EClass.rnd(3) == 0 ? "pianist" : EClass.rnd(2) == 0 ? "farmer" : "tourist"));
            }

            internal override void ApplyRandomAbility(Chara c)
            {
                var summonSpell1 = EClass.sources.elements.rows.Where(r => r.alias.StartsWith("SpSummon") && r.chance > 0).RandomItem().id;
                var summonSpell2 = EClass.sources.elements.rows.Where(r => r.alias.StartsWith("SpSummon") && r.id != summonSpell1 && r.chance > 0).RandomItem().id;
                var elementId = EClass.sources.elements.rows.Where(r => r.categorySub == "eleAttack" && r.chance > 0).RandomItem().id - 910;

                c.ability.Add(summonSpell1, 50, false);
                c.ability.Add(summonSpell2, 50, false);

                if (EClass.rnd(2) == 0)
                {
                    // 具象魔法
                    c.ability.Add(50600 + elementId, 50, false);
                }
                else
                {
                    // ビット魔法
                    c.ability.Add(51100 + elementId, 50, false);
                }
            }
        }
        #endregion

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
                    SpawnEnemy(dangerLv, (i % 3 == 0 ? "vanguard" : (i % 3 == 1 ? "rearguard" : "supporter")));
                }
                AggroEnemy(50);
                EClass._zone.SetBGM(102);
                max = enemies.Count;
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
            return Mathf.Max(base.quest.DangerLv, 1);
        }


        internal virtual int CalcNumberOfEnemies()
        {
            return 3 + (base.quest.difficulty * 4) / 7; // 1～2:3 3:4 4～5:5 6～7:6
        }

        internal virtual void SpawnEnemy(int dangerLv, string role)
        {
            var spawnPoint = EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false, minRadius: 3);

            Type specialtyType = null;
            switch (role)
            {   
                case "vanguard":
                    specialtyType = VanguardSpecialtyTypeResolver.Values.RandomItem();
                    break;
                case "rearguard":
                    specialtyType = RearguardTypeResolver.Values.RandomItem();
                    break;
                case "supporter":
                    specialtyType = SupporterTypeResolver.Values.RandomItem();
                    break;
                default:
                    specialtyType = VanguardSpecialtyTypeResolver.Values.RandomItem();
                    break;
            }

            var enemy = CreateEnemy(dangerLv, Activator.CreateInstance(specialtyType, true) as Specialty);

            EClass._zone.AddCard(enemy, spawnPoint);

            enemies.Add(enemy.uid);
        }

        internal virtual Chara CreateEnemy(int dangerLv, Specialty specialty = null)
        {
            int generateLv = Mathf.Max(dangerLv * 3 / 2, 5);

            var charaBlueprint = new CardBlueprint
            {
                rarity = Rarity.Legendary,
                lv = generateLv,
            };

            CardBlueprint.Set(charaBlueprint);

            var createdChara = CharaGen.Create((EClass.rnd(10) == 0 ? "adv_fairy" : "adv"), generateLv);

            if (specialty != null)
            {
                specialty.Apply(createdChara);
            }

            // 敵対設定
            createdChara.c_bossType = BossType.Boss;
            createdChara.c_originalHostility = Hostility.Enemy;
            createdChara.hostility = Hostility.Enemy;

            // レベル調整、低層帯はLvの影響を受けやすいので減らす
            int charaLv = (dangerLv < 100) ? dangerLv * 4 / 5 :
                          (dangerLv < 300) ? dangerLv :
                          (dangerLv < 500) ? dangerLv * 5 / 4 : generateLv;
            createdChara.SetLv(charaLv);

            createdChara.RestockEquip(true);
            createdChara.AddThing(createdChara.MakeGene((EClass.rnd(2) == 0) ? DNA.Type.Superior : DNA.Type.Default));

            return createdChara;
        }
    }
}
