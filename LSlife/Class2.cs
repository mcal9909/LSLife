using System;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;
using System.Drawing;

namespace MissionTutorial
{
    public class Main : Script
    {
        Ped somePed;
        int MissionIndex = 1;
        bool MissionActive = false;
        Blip missionblip;
        Vector3 CratePostion = new Vector3(152.2624f, -1033.266f, 28.33942f);
        Vector3 CratePostion2 = new Vector3(147.0731f, -1044.892f, 28.3681f);

        public Main()
        {
            UI.Notify("Made By ~r~Bubbaj0423");
            Tick += onTick;
            KeyDown += onKeyDown;
        }
        private void onTick(object sender, EventArgs e)
        {
            if (MissionActive)
            {
                switch (MissionIndex)
                {
                    case 0:
                        {
                            missionblip = World.CreateBlip(CratePostion);
                            if (missionblip.Exists())
                            {
                                missionblip.Sprite = (BlipSprite)108;
                                missionblip.Color = BlipColor.Green;
                                missionblip.Name = "Safe";
                                missionblip.ShowRoute = true;
                            }
                            MissionIndex = 50;
                        }
                        break;
                    case 10:
                        {
                            if (Game.Player.Character.Position.DistanceTo(CratePostion) < 10f)
                            {
                                if (missionblip.Exists())
                                {
                                    missionblip.ShowRoute = false;
                                }
                                //UI.Notify("Rob The Safe");
                                MissionIndex = 20;
                            }

                        }
                        break;
                    case 20:
                        {
                            World.DrawMarker(MarkerType.VerticalCylinder, CratePostion, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(1.5f, 1.5f, 0.5f), Color.FromArgb(0, 160, 160));
                            if (Game.Player.Character.Position.DistanceTo(CratePostion) < 1.5f)
                            {
                                DisplayHelpTextThisFrame("Press E To Start The ~g~Flecca Bank Heist");
                                if (Game.IsControlJustPressed(2, GTA.Control.Context))
                                {
                                    MissionIndex = 30;
                                }
                            }

                        }
                        {
                            

                        }
                        break;
                    case 30:
                        {
                            if (somePed == null) somePed = World.CreatePed(PedHash.SecuroGuardMale01, new Vector3(147.5546f, -1044.217f, 28.36803f));
                            if (somePed != null)
                            {
                                somePed.Weapons.Give(WeaponHash.AssaultRifle, 1000, true, true);
                                int EnemyRelationShipGroup = Function.Call<int>(Hash.GET_HASH_KEY, "HATES_PLAYER");
                                somePed.RelationshipGroup = EnemyRelationShipGroup;
                                //somePed.Task.FightAgainst(Game.Player.Character);
                                somePed.Task.GuardCurrentPosition();
                                MissionIndex = 40;
                                
                            }                            
                        }
                        break;
                    case 40:
                        {
                            if (somePed.IsDead)
                            {
                                somePed.MarkAsNoLongerNeeded();
                                somePed.IsPersistent = false;
                                somePed = null;
                                MissionIndex = 50;
                            }                            
                        }
                        break;
                    case 50:
                        {
                            World.DrawMarker(MarkerType.VerticalCylinder, CratePostion2, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(1.5f, 1.5f, 0.5f), Color.FromArgb(0, 160, 160));
                            if (Game.Player.Character.Position.DistanceTo(CratePostion2) < 1.5f)
                            {
                                DisplayHelpTextThisFrame("Press E To Rob The ~g~Safe");
                                if (Game.IsControlJustPressed(2, GTA.Control.Context))
                                {
                                    MissionIndex = 60;
                                }
                            }
                        }
                        break;
                    case 60:
                        {
                            UI.Notify("Robbing The Safe...");
                            Game.Player.Character.FreezePosition = true;
                            Script.Wait(3000);
                            Game.Player.Character.FreezePosition = false;
                            MissionIndex = 70;
                        }
                        break;
                    case 70:
                        {
                            ChooseRandomReward();
                            if (missionblip.Exists())
                            {
                                missionblip.Alpha = 0;
                                missionblip.Remove();
                            }
                            MissionActive = false;
                            MissionIndex = -1;


                        }
                        break;
                }
            }
        }
        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad1)
            {
                MissionIndex = 0;
                MissionActive = true;
            }
            //if (e.KeyCode == Keys.NumPad7)
            //{
            //    float xpos = Game.Player.Character.Position.X;
            //    float ypos = Game.Player.Character.Position.X;
            //    float zpos = Game.Player.Character.Position.X;
            //    float heading = Game.Player.Character.Heading;
            //    UI.Notify("X: " + xpos + " Y: " + ypos + " Z: " + zpos + " H: " + heading);
            //}
        }
        void DisplayHelpTextThisFrame(string text) // Credit to JediJosh For Making This Function. 
        {
            Function.Call(Hash._SET_TEXT_COMPONENT_FORMAT, "STRING");
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, text);
            Function.Call(Hash._0x238FFE5C7B0498A6, 0, 0, 1, -1);
        }

        void ChooseRandomReward()
        {
            int randomchoice = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 100);
            if (randomchoice < -30)
            {
                UI.Notify("You Found Nothing Of Value");
            }
            if (randomchoice > 30)
            {
                int randommoney = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 100000);
                Game.Player.Money += randommoney;
                UI.Notify("You Have Rob The Bank Now ~r~Lose The Cops! You Earn $" + randommoney);
            }
        }
    }
}

