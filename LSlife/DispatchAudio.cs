using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using NAudio;
using GTA;
using GTA.Native;
using GTA.Math;
using NativeUI;
using iFruitAddon2;
using System.IO;
using Control = GTA.Control;
using static LSlife.LsFunctions;

namespace LSlife
{
    static class DispatchAudio
    {
        static bool DEBUG = true;
        public static bool Started = false;
        static private bool cAudioFinished = false;
        static private int cAudio = -1;
        private enum DispatchStates { None, AllUnits, AssRequire, DrugDeal}
        static private DispatchStates dispatchState = DispatchStates.None;

        public static void Play()
        {            
            switch(dispatchState)
            {
                case DispatchStates.None:
                    if(DEBUG) UI.Notify("01_attention_all_units_gen");
                    Audio.SetAudioFlag(AudioFlag.PoliceScannerDisabled, false);
                    Audio.SetAudioFlag(AudioFlag.OnlyAllowScriptTriggerPoliceScanner, false);
                    cAudio = Audio.PlaySoundFrontend("01_attention_all_units_gen", "ATTENTION_ALL_UNITS_GENERIC_01");                    
                    Started = false;
                    break;
                case DispatchStates.AllUnits:
                    if (Audio.HasSoundFinished(cAudio))
                    {
                        if (DEBUG) UI.Notify("01_assistance_required");
                        Audio.ReleaseSound(cAudio);
                        cAudio = AssRequire();
                        dispatchState = DispatchStates.AssRequire;
                    }
                    break;
                case DispatchStates.AssRequire:
                    if(Audio.HasSoundFinished(cAudio))
                    {
                        if (DEBUG) UI.Notify("01_crime_drug_deal");
                        Audio.ReleaseSound(cAudio);
                        cAudio = DrugDeal();
                        dispatchState = DispatchStates.DrugDeal;
                    }
                    break;
                case DispatchStates.DrugDeal:
                    if(Audio.HasSoundFinished(cAudio))
                    {
                        Audio.ReleaseSound(cAudio);
                        dispatchState = DispatchStates.None;
                        Started = false;
                    }
                    break;
            }            
        }
        static int AllUnits()
        {
            return Audio.PlaySoundFrontend("01_attention_all_units_gen", "ATTENTION_ALL_UNITS_GENERIC_01");
        }
        static int AssRequire()
        {
            return Audio.PlaySoundFrontend("01_assistance_required");
        }
        static int DrugDeal()
        {
            return Audio.PlaySoundFrontend("01_crime_drug_deal");
        }
    }
}
