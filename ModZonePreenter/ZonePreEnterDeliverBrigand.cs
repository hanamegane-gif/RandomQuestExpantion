using System.Collections.Generic;

namespace RandomQuestExpantion.ModZonePreenter
{
    class ZonePreEnterDeliverBrigand : ZonePreEnterEvent
    {
        internal virtual HashSet<string> SpawnCandidateList { get; } = new HashSet<string>
        {
            "merc", "merc_archer", "merc_mage", "merc_warrior", "dog_hound"
        };

        public Thing DemandedThing;

        public ZonePreEnterDeliverBrigand(Thing demanded)
        {
            DemandedThing = demanded;
        }

        public override void Execute()
        {
            Chara leader = null;
            var randomPointInRadius = EClass.pc.pos.GetRandomPointInRadius(2, 5, requireLos: false, allowChara: false);
            int spawnNum = EClass.rndHalf(12);
            for (int i = spawnNum; i > 0; i--)
            {
                var randomPointInRadius2 = randomPointInRadius.GetRandomPointInRadius(1, 4, requireLos: false, allowChara: false);
                if (randomPointInRadius2 != null)
                {
                    var chara = EClass._zone.SpawnMob(randomPointInRadius2, SpawnSetting.Mob(SpawnCandidateList.RandomItem()));
                    chara.hostility = chara.c_originalHostility = Hostility.Enemy;
                    if (i == 1)
                    {
                        leader = chara;
                    }
                }
            }

            GameLang.refDrama1 = DemandedThing.GetName(NameStyle.Full);
            leader.ShowDialog("RQXDialog", "assassin");
        }
    }
}
