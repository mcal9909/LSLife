using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using GTA.Native;
using GTA.Math;
using NativeUI;
using iFruitAddon2;
using System.IO;
using Control = GTA.Control;
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
            this.vehicle = World.CreateVehicle(this.vehModel, GenerateSpawnPos(pos.Around(100), Nodetype.Road, false));
            this.vehicle.IsPersistent = true;
            this.vehicle.AddBlip();
            this.vehicle.CurrentBlip.IsFriendly = true;
            this.ped = World.CreatePed(pedModel, this.vehicle.Position);
            this.ped.IsPersistent = true;
            this.ped.SetIntoVehicle(this.vehicle, VehicleSeat.Driver);
            Function.Call(Hash.SET_DRIVER_ABILITY, this.ped.Handle, 100f);
            Function.Call(Hash._SET_EXCLUSIVE_DRIVER, this.vehicle.Handle, this.ped.Handle);
            this.destination = pos;
            this.ped.Task.DriveTo(this.vehicle, this.destination, 2f, 25f, (int)DrivingStyle.SometimesOvertakeTraffic);
            this.state = States.Driving;
            this.ped.StaysInVehicleWhenJacked = true;
            this.ped.BlockPermanentEvents = true;
            maxSpeed = Function.Call<float>(Hash._GET_VEHICLE_MAX_SPEED, this.vehicle) * 0.8f;
        }
    }
}
