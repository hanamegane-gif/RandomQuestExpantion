using HarmonyLib;
using Newtonsoft.Json;
using RandomQuestExpantion.General;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngineInternal;

namespace RandomQuestExpantion.ModQuestEvent
{
    internal class ZoneEventMusicMonster : ZoneEventMusic
    {
        enum Voltage
        {
            Max = 100,
            VeryHigh = 75,
            High = 50,
            Moderate = 25,
            Low = 0,
            Annoy = -20,
            Messed = -50,
            Fucked = -75,
            Worst = -100,
        }

        [JsonProperty]
        public int AudienceVoltage = 15;

        [JsonProperty]
        public Queue<int> recentScores = new Queue<int>(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0});

        public string GetVoltageStringRaw => AudienceVoltage switch
        {
            >= (int)Voltage.VeryHigh => "byakko_mod_music_mood_veryhigh",
            >= (int)Voltage.High => "byakko_mod_music_mood_high",
            >= (int)Voltage.Moderate => "byakko_mod_music_mood_moderate",
            >= (int)Voltage.Low => "byakko_mod_music_mood_low",
            >= (int)Voltage.Annoy => "byakko_mod_music_mood_annoy",
            >= (int)Voltage.Messed => "byakko_mod_music_mood_messed",
            >= (int)Voltage.Fucked => "byakko_mod_music_mood_fucked",
            _ => "byakko_mod_music_mood_fucked"
        };

        public string GetVoltageString => GetVoltageStringRaw.lang();


        internal virtual HashSet<string> SpawnRaceCandidatesList => new HashSet<string>
        {
            "yeek",
        };

        public override string TextWidgetDate =>
            "byakko_mod_status_monster_music".lang(GetVoltageString) +
            "byakko_mod_progress_timer".lang((TimeLimit - minElapsed).ToString()) +
            ((TimeLimit - minElapsed <= 30) ? "  " + "end_soon".lang() : "");

        public override void OnVisit()
        {
            if (EClass.game.isLoading)
            {
                return;
            }

            int difficulty = questMusic.difficulty;
            int maxLv = difficulty * 5 + 2;
            if (difficulty > 2)
            {
                maxLv += (difficulty - 2) * 10;
            }

            var spawnableRaces = SelectSpawnRace();

            SetFieldPiece(spawnableRaces);

            for (int i = 0; i < 15; i++)
            {
                Point randomPoint = EClass._map.bounds.GetRandomPoint();
                if (randomPoint.HasChara || randomPoint.HasBlock)
                {
                    continue;
                }
                Chara chara = CreateBossChara(spawnableRaces);
                EClass._zone.AddCard(chara, randomPoint);
            }

            for (int i = 0; i < 50; i++)
            {
                Point randomPoint = EClass._map.bounds.GetRandomPoint();
                if (!randomPoint.HasChara && !randomPoint.HasBlock)
                {
                    Chara chara = CreateAudienceChara(spawnableRaces);
                    EClass._zone.AddCard(chara, randomPoint);
                }
            }

            foreach (Thing thing in EClass._map.things)
            {
                thing.isNPCProperty = true;
            }
        }

        public override void OnCharaDie(Chara c)
        {
            if (!c.IsPCFactionOrMinion)
            {
                AudienceVoltage--;
                sanitiseVoltage();
            }
        }

        public override void _OnTickRound() 
        {
            recentScores.Dequeue();
            recentScores.Enqueue(this.questMusic.score);
            this.questMusic.score = 0;

            int roundScore = recentScores.Sum();

            AudienceVoltage--;

            if (roundScore < 0)
            {
                AudienceVoltage--;
                AudienceVoltage += Mathf.Clamp(-1 * (int)Mathf.Sqrt(Mathf.Abs(roundScore) / 50) + 1, -3, 0);
            }

            if (roundScore > 40)
            {
                AudienceVoltage++;
                AudienceVoltage += Mathf.Clamp((int)Mathf.Sqrt(roundScore / 20), 0, 3);
            }

            // PCが演奏中ならスコアに関係なく30%の確率で+1(高Lv帯の運ゲー防止)
            // とりあえず演奏していればある程度耐えられるようにする
            if (EClass.pc.ai is AI_PlayMusic && EClass.rnd(10) < 3)
            {
                AudienceVoltage++;
            }

            YUGAMINEENA();
            AggroEnemy();
            sanitiseVoltage();
        }

        public override ZoneInstance.Status OnReachTimeLimit()
        {
            string resultRawString = AudienceVoltage switch
            {
                >= (int)Voltage.VeryHigh => "great",
                >= (int)Voltage.High => "good",
                >= (int)Voltage.Moderate => "",
                >= (int)Voltage.Low => "",
                >= (int)Voltage.Annoy => "bad",
                >= (int)Voltage.Messed => "bad",
                >= (int)Voltage.Fucked => "worse",
                _ => "worse"
            };

            Msg.Say("byakko_mod_music_party_end");
            if (resultRawString != "")
            {
                Msg.Say("byakko_mod_music_party_end_" + resultRawString);
            }
            Msg.Say("party_end2", questMusic.sumMoney.ToString() ?? "");

            if (AudienceVoltage >= (int)Voltage.High)
            { 
                quest.bonusMoney = questMusic.sumMoney * AudienceVoltage / 200;
            }
            else if(AudienceVoltage <= (int)Voltage.Fucked)
            {
                quest.bonusMoney = quest.rewardMoney * Math.Abs(AudienceVoltage) / 200;
            }

            return ZoneInstance.Status.Success;
        }

        internal virtual void SetFieldPiece(HashSet<String> spawnableRaces)
        {
            var genBounds = GenBounds.Create(EClass._zone);
            genBounds.marginPartial = 1;

            // インスタンスマップにはタイルの床を敷き詰めた
            genBounds.FuncCheckEmpty = (Cell cell) => cell.sourceFloor.id == 66;

            Action<PartialMap, GenBounds> onCreate = delegate (PartialMap p, GenBounds b)
            {
                var list = b.ListEmptyPoint();
                int numAudience = EClass.rndHalf(list.Count * 2) / 4;
                for (int i = 0; i < numAudience; i++)
                {
                    if (list.Count == 0)
                    {
                        break;
                    }
                    var spawnPoint = list.RandomItem();
                    var extraChara = CreateAudienceChara(spawnableRaces);
                    EClass._zone.AddCard(extraChara, spawnPoint);
                    list.Remove(spawnPoint);
                }
            };

            for (int i = 0; i < 50; i++)
            {
                string pieceType = "music_noble";
                ModMapPiece.TryAddMapPiece(genBounds, pieceType, onCreate);
            }

            foreach (var thing in EClass._map.things)
            {
                thing.isNPCProperty = true;
            }
        }

        internal virtual HashSet<String> SelectSpawnRace()
        {
            var canGenerate = new HashSet<string>();

            foreach (var race in SpawnRaceCandidatesList)
            {
                if (EClass.sources.charas.rows.Any(r => 
                    r.race == race && 
                    r.chance > 0 && 
                    r.hostility == "" && 
                    r.LV <= quest.DangerLv && 
                    !r.elements.Contains(FEAT.featMurderer) &&
                    !r.actCombat.Any(a => a.StartsWith("SpGate")))
                )
                {
                    canGenerate.Add(race);
                }
            }

            return canGenerate.OrderBy(_ => Guid.NewGuid()).Take(5).ToHashSet();
        }

        // 通常の観客を作る
        internal virtual Chara CreateAudienceChara(HashSet<String> spawnableRaces)
        {
            int generateLv = (quest.DangerLv * 17 / 20) + EClass.rnd(quest.DangerLv * 3 / 20 + 1) + 
                             (quest.DangerLv * quest.difficulty * quest.difficulty / 1000);
            string race = spawnableRaces.RandomItem();
            var charaId = EClass.sources.charas.rows.Where(r =>
                    r.race == race &&
                    r.quality < 3 && 
                    r.chance > 0 &&
                    r.hostility == "" &&
                    r.LV <= quest.DangerLv &&
                    !r.elements.Contains(FEAT.featMurderer) &&
                    !r.actCombat.Any(a => a.StartsWith("SpGate")))
                .RandomItem().id;
            var charaBlueprint = new CardBlueprint
            {
                lv = generateLv,
            };

            CardBlueprint.Set(charaBlueprint);

            var createdChara = CharaGen.Create(charaId);
            //createdChara.SetLv(generateLv);
            createdChara.c_originalHostility = Hostility.Neutral;
            createdChara.hostility = Hostility.Neutral;
            return createdChara;
        }

        // ボスをつくる　通常の演奏依頼のユニークのかわり
        internal virtual Chara CreateBossChara(HashSet<String> spawnableRaces)
        {
            int generateLv = (quest.DangerLv * 17 / 20) + EClass.rnd(quest.DangerLv * 4 / 20 + 1) +
                             (quest.DangerLv * quest.difficulty * quest.difficulty / 1000); string race = spawnableRaces.RandomItem();
            var charaId = EClass.sources.charas.rows.Where(r =>
                    r.race == race &&
                    r.chance > 0 &&
                    r.hostility == "" &&
                    r.LV <= quest.DangerLv &&
                    !r.elements.Contains(FEAT.featMurderer) &&
                    !r.actCombat.Any(a => a.StartsWith("SpGate")))
                .RandomItem().id;
            var charaBlueprint = new CardBlueprint
            {
                rarity = Rarity.Legendary,
                lv = generateLv,
            };
            CardBlueprint.Set(charaBlueprint);

            var createdChara = CharaGen.Create(charaId);
            createdChara.c_originalHostility = Hostility.Neutral;
            createdChara.hostility = Hostility.Neutral;
            return createdChara;
        }

        private void YUGAMINEENA()
        {
            if (AudienceVoltage >= (int)Voltage.Annoy)
            {
                return;
            }

            var audiences = EClass._zone.map.charas.Where(r => !r.IsPCFactionOrMinion && r.c_originalHostility != Hostility.Enemy);

            foreach (var chara in audiences)
            {
                if (EClass.rnd(10000) < AudienceVoltage * AudienceVoltage)
                {
                    int charaLv = CalcHostileBaseLv(chara);
                    chara.SetLv((chara.rarity >= Rarity.Legendary) ? charaLv * 3 / 2 : charaLv);
                    chara.hp = chara.MaxHP;
                    chara.c_originalHostility = Hostility.Enemy;
                    chara.hostility = Hostility.Enemy;
                    chara.ShowEmo(Emo.angry);
                }
            }
        }

        private new void AggroEnemy()
        {
            if (AudienceVoltage >= (int)Voltage.Messed)
            {
                return;
            }

            int chance = AudienceVoltage * AudienceVoltage / 300;
            var PCParty = EClass._map.charas.Where(c => c.IsPCFactionOrMinion).ToList();

            // タレットをターゲットにすると回り込むように移動するためめんどくさくなる

            foreach (var chara in EClass._map.charas.Where(c => !c.IsPCFactionOrMinion && !c.IsInCombat && c.c_originalHostility == Hostility.Enemy))
            {
                if (chance < 100 && EClass.rnd(100) >= chance)
                {
                    continue;
                }
                chara.SetEnemy(PCParty.RandomItem());
                chara.SetAIAggro();
            }
        }

        private int CalcHostileBaseLv(Chara c)
        {
            int fameLv = EClass.pc.FameLv * 3 / 4;
            int charaBaseLv = EClass.sources.charas.rows.Where(r => r.id == c.id).First().LV;
            int voidMul = (fameLv < 51) ? 0 : fameLv / 50;
            int charaLv = (charaBaseLv + ((voidMul > 0) ? 50 : 0)) * voidMul;

            return Mathf.Max(EClass.pc.elements.ValueWithoutLink(questMusic.partyLv), charaLv);
        }

        private void sanitiseVoltage()
        {
            AudienceVoltage = Mathf.Clamp(AudienceVoltage, (int)Voltage.Worst, (int)Voltage.Max);
        }
    }
}
