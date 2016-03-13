using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace OakBot
{
    public class BackgroundTasks
    {
        private DateTime activated;
        private double secondsPassed;

        private int intervalPoints;
        private int intervalSave;

        public BackgroundTasks(int intervalPoints, int intervalSave)
        {
            this.intervalPoints = intervalPoints;
            this.intervalSave = intervalSave;
        }

        // For running in seperate thread with auto sleep balancing
        internal void Run()
        {
            // Timestamp of start of the thread
            activated = DateTime.UtcNow;

            while (true)
            {
                // Sleep for 60sec auto compensated with calculating and starting tasks.
                Thread.Sleep(Convert.ToInt32(60000 - Math.Floor(DateTime.UtcNow.Subtract(activated).TotalMilliseconds % 60000)));

                // Get total time difference in seconds
                secondsPassed = Math.Floor(DateTime.UtcNow.Subtract(activated).TotalSeconds);

                // Activate task point distribution
                if (secondsPassed % intervalPoints == 0)
                {
                    new Task(() =>
                    {
                        Trace.WriteLine(DateTime.UtcNow.ToString("o") + " -> Executing Point interval set to: " + intervalPoints);
                    }).Start();
                }

                // Activate task save
                if (secondsPassed % intervalSave == 0)
                {
                    new Task(() =>
                    {
                        Trace.WriteLine(DateTime.UtcNow.ToString("o") + " -> Executing Save interval set to: " + intervalSave);
                    }).Start();
                }
            }
        }
    }
}