using GTA;
using System.Collections.Generic;
using System.Linq;

namespace LSlife
{
    public class DriveByHandler
    {
        public List<DriveBy> GetDriveBies = new List<DriveBy>();
        public DriveByHandler()
        {

        }
        public void OnTick()
        {
            if (GetDriveBies.Count > 0)
            {
                foreach (DriveBy driveBy in GetDriveBies.ToList())
                {
                    if (driveBy.Dead)
                    {
                        GetDriveBies.Remove(driveBy);
                    }
                    else
                    {
                        driveBy.OnTick();
                    }
                }
            }
        }

        public void AddDriveBy(VehicleClass _class, int _amount, Ped _target)
        {
            GetDriveBies.Add(new DriveBy(_class, _amount, _target));
        }

        public int AmountOfDriveBy()
        {
            return GetDriveBies.Count;
        }

        public List<Ped> GetAllPeds()
        {
            Ped[] p = { };
            if (GetDriveBies.Count > 0)
            {
                foreach (DriveBy driveBy in GetDriveBies)
                {
                    foreach (Ped ped in driveBy.Peds.Where(r => r != null && r.Exists()))
                    {
                        p.Append(ped);
                    }
                }
            }
            if (p.Count() > 0) return p.ToList();
            else return null;
        }

        public void CleanDriveBys()
        {
            foreach (DriveBy d in GetDriveBies.ToList())
            {
                d.CleanDriveBy();
                GetDriveBies.Remove(d);
            }
        }

    }
}
