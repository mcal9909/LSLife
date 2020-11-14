using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using static GTA.Script;


namespace LSlife
{
    class EnemyHandler
    {
        List<Vector3> SpawnLocations = new List<Vector3>
        {
           new Vector3 (-131.695f, -1509.14f, 34.10052f),
           new Vector3 (-161.0194f, -1515.375f, 33.68655f),
           new Vector3 (-172.2392f, -1492.894f, 32.55872f)
        };
        Vector3 ClosePos;
        List<Ped> Enemies = new List<Ped>();
        int MaxPeds = 15;
        int TimeBetweenSpawns = 500;
        int Time;
        public bool Started { get; private set; } = false;
        List<PedHash> pedHashes = new List<PedHash>
        {
            PedHash.AvonGoon,
            PedHash.Juggernaut01M
        };
        Random Rand = new Random();

        public EnemyHandler()
        {

        }

        public void Ontick()
        {
            if (Started)
            {
                if (Game.GameTime > Time + TimeBetweenSpawns && Enemies.Count < MaxPeds)
                {
                    int index = -1;
                    bool posFound = false;
                    ClosePos = SpawnLocations[0];
                    foreach (Vector3 v in SpawnLocations)
                    {
                        if (v.DistanceTo(Game.Player.Character.Position) < ClosePos.DistanceTo(Game.Player.Character.Position))
                        {
                            ClosePos = v;
                        }
                    }
                    while (!posFound)
                    {
                        index = Rand.Next(0, SpawnLocations.Count);
                        if (SpawnLocations[index] != ClosePos)
                        {
                            posFound = true;
                        }
                    }
                    int pedIndex = Rand.Next(0, 2);
                    Ped enemy = World.CreatePed(pedHashes[pedIndex], SpawnLocations[index]);
                    while (enemy == null)
                    {
                        Wait(0);
                    }
                    switch (pedIndex)
                    {
                        case 0:
                            enemy.Weapons.Give(WeaponHash.CarbineRifle, 1000, true, true);
                            Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, enemy, 2);
                            break;
                        case 1:
                            enemy.Weapons.Give(WeaponHash.Minigun, 10000, true, true);
                            enemy.CanSufferCriticalHits = false;
                            Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, enemy, 3);
                            break;
                    }
                    Function.Call(Hash.SET_PED_COMBAT_ABILITY, enemy, 100);
                    enemy.FiringPattern = FiringPattern.FullAuto;
                    enemy.CanWrithe = false;
                    enemy.Armor = 100;
                    //enemy.RelationshipGroup = enemyTeam;
                    enemy.AlwaysKeepTask = true;
                    enemy.Task.FightAgainst(Game.Player.Character);
                    enemy.AddBlip();
                    enemy.CurrentBlip.IsFriendly = false;
                    Enemies.Add(enemy);
                    Time = Game.GameTime;
                }

            }
            CheckPeds();
        }

        public void Start()
        {
            Started = true;
        }

        public void Stop()
        {
            Started = false;
        }

        void CheckPeds()
        {
            if (Enemies.Count > 0)
            {
                foreach (Ped p in Enemies.ToList())
                {
                    if (p == null || !p.Exists())
                    {
                        Enemies.Remove(p);
                    }
                    else if (p.IsDead)
                    {
                        p.CurrentBlip.Remove();
                        Enemies.Remove(p);
                    }
                }
            }
        }

        void RemoveEnemies()
        {
            foreach (Ped p in Enemies.ToList())
            {
                p.CurrentBlip.Remove();
                Enemies.Remove(p);
            }
        }
    }
}
