using GTA;

namespace LSlife
{
    public class LsTimer
    {
        int Time = 0;
        int TotalTime = 0;
        bool TimeOut = false;
        public bool StartTimer = false;
        private int lastTime;

        public LsTimer(int time, bool timeOut, bool start)
        {
            TotalTime = time;
            TimeOut = timeOut;
            lastTime = Game.GameTime;
            StartTimer = start;

        }
        #region Timer Funtions
        public void Tick()
        {
            if (StartTimer && Game.GameTime > lastTime + 1000)
            {
                lastTime = Game.GameTime;
                if (!Finished())
                {
                    Time++;
                }
                else
                {
                    if (TimeOut)
                    {
                        Time = 0;
                    }
                }
            }
        }
        public bool Finished()
        {
            if (Time >= TotalTime) return true;
            else return false;
        }
        public int Remaining()
        {
            if (Time < TotalTime) return Time - TotalTime;
            else return 0;
        }
        public int Elapsed()
        {
            return Time;
        }
        public void Start()
        {
            StartTimer = true;
            lastTime = Game.GameTime;
        }
        public void Stop()
        {
            StartTimer = false;
        }
        public void Reset()
        {
            Time = 0;
            lastTime = Game.GameTime;
        }
        #endregion
    }
}
