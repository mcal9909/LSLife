using GTA;
using System.Collections.Generic;
using System.Linq;
using static LSlife.LSL;

namespace LSlife
{
    public class LsLPeds
    {
        public List<Ped> GetPeds { get; private set; }
        int ptime;
        int pdelay = 5000;

        public LsLPeds()
        {
            GetPeds = World.GetNearbyPeds(Game.Player.Character, 200).Where(p => CheckIfPedUsed(p, true, true) == false).ToList();
            ptime = Game.GameTime;
        }

        public void Ontick()
        {
            if (Game.GameTime > ptime + pdelay)
            {
                ptime = Game.GameTime;
                foreach (Ped p in World.GetNearbyPeds(Game.Player.Character, 200).Where(p => !CheckIfPedUsed(p, true, true) && !GetPeds.Contains(p)))
                {
                    GetPeds.Add(p);
                }
            }
            foreach (Ped p in GetPeds.ToList())
            {
                if (p == null || !p.Exists()) GetPeds.Remove(p);
                //else if (Function.Call<bool>(Hash.IS_ENTITY_VISIBLE, p)) Function.Call(Hash.SET_ENTITY_VISIBLE, p, false, 0);
            }
        }
    }
}
