using RandomQuestExpantion.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomQuestExpantion.ModQuestEvent
{
    internal class ZoneEventMusicMonsterFilth : ZoneEventMusicMonster
    {
        internal override HashSet<string> SpawnRaceCandidatesList => new HashSet<string>
        {
            "succubus",
            "goblin",
            "kobolt",
            "orc",
            "lizardman",
            "harpy",
            "mermaid",
            "yeek",
        };

        internal override void SetFieldPiece(HashSet<String> spawnableRaces)
        {
            var genBounds = GenBounds.Create(EClass._zone);
            genBounds.marginPartial = 1;

            // インスタンスマップにはタイルの床を敷き詰めた
            genBounds.FuncCheckEmpty = (Cell cell) => cell.sourceFloor.id == 99;

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
                string pieceType = "music_filth";
                ModMapPiece.TryAddMapPiece(genBounds, pieceType, onCreate);
            }

            foreach (var thing in EClass._map.things)
            {
                thing.isNPCProperty = true;
            }
        }
    }
}
