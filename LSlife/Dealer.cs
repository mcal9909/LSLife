using GTA;
using GTA.Math;
using GTA.Native;
using static GTA.Script;
using static LSlife.LsFunctions;
using static LSlife.LSL;

namespace LSlife
{
    public class Dealer
    {
        public enum States { Driving, Ariving, Leaving, Arrived, Faster, Fast, Slower }
        public float maxSpeed;
        public Vector3 destination;
        public bool doneTutorial = false;
        public States state;
        public Ped ped;
        public Vector3 pos;
        private Model vehModel;
        public Vehicle vehicle;


        public Dealer(Model pedModel, Vector3 pos, Model vehModel, PedGroup group)
        {
            this.pos = pos;
            this.vehModel = vehModel;
            this.vehicle = World.CreateVehicle(this.vehModel, GenerateSpawnPos(pos.Around(80), Nodetype.Road, false));
            while (vehicle == null)
            {
                Wait(0);
            }
            this.vehicle.IsPersistent = true;

            AddBlip(vehicle, BlipColor.Blue, "Zee's Driver", true, false);

            this.ped = World.CreatePed(pedModel, this.vehicle.Position);
            while (ped == null)
            {
                Wait(0);
            }
            this.ped.IsPersistent = true;
            this.ped.SetIntoVehicle(this.vehicle, VehicleSeat.Driver);
            Function.Call(Hash.SET_DRIVER_ABILITY, this.ped.Handle, 100f);
            Function.Call(Hash._SET_EXCLUSIVE_DRIVER, this.vehicle.Handle, this.ped.Handle);
            this.destination = World.GetNextPositionOnStreet(pos, true);
            this.ped.Task.DriveTo(this.vehicle, this.destination, 5f, 25f, (int)DrivingStyle.SometimesOvertakeTraffic);
            this.state = States.Driving;
            this.ped.StaysInVehicleWhenJacked = true;
            this.ped.BlockPermanentEvents = true;
            maxSpeed = Function.Call<float>(Hash._GET_VEHICLE_MAX_SPEED, this.vehicle) * 0.8f;
        }
    }
}
