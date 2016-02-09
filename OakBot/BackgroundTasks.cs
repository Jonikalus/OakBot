using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OakBot
{
    public class BackgroundTasks
    {
        private DateTime activated;
        private int intervalPoints;
        private int intervalSave;

        private double secondsPassed;

        public BackgroundTasks(int intervalPoints, int intervalSave)
        {
            this.intervalPoints = intervalPoints;
            this.intervalSave = intervalSave;
            activated = DateTime.UtcNow;
        }

        // For running in seperate thread with auto sleep balancing
        //internal void Run()
        //{
        //
        //    // Timestamp of start of the thread
        //    activated = DateTime.UtcNow;
        //
        //    while (true)
        //    {
        //
        //        // Sleep for 60sec auto compensated with calculating and starting tasks.
        //        Thread.Sleep(Convert.ToInt32(60000 - Math.Floor(DateTime.UtcNow.Subtract(activated).TotalMilliseconds % 60000)));
        //
        //        // Get total time difference in seconds
        //        secondsPassed = Math.Floor(DateTime.UtcNow.Subtract(activated).TotalSeconds);
        //
        //        // Activate task point distribution
        //        if ( secondsPassed % intervalPoints == 0)
        //        {
        //            new Task(() => {
        //
        //                Trace.WriteLine(DateTime.UtcNow.ToString("o") + " -> Executing Point interval set to: " + intervalPoints);
        //
        //            }).Start();
        //        }
        //
        //        // Activate task save
        //        if (secondsPassed % intervalSave == 0)
        //        {
        //            new Task(() => {
        //
        //                Trace.WriteLine(DateTime.UtcNow.ToString("o") + " -> Executing Save interval set to: " + intervalSave);
        //
        //            }).Start();
        //        }
        //
        //    }
        //
        //}

        // Run with timer
        public void Run()
        {
            // Get total time difference in seconds
            secondsPassed = Math.Floor(DateTime.UtcNow.Subtract(activated).TotalSeconds);

            Trace.WriteLine(DateTime.UtcNow.ToString("o") + " : " + secondsPassed);

            // Activate task point distribution
            if ( secondsPassed % intervalPoints == 0)
            {
                new Task(() => {
            
                    Trace.WriteLine(DateTime.UtcNow.ToString("o") + " -> Executing Point interval set to: " + intervalPoints);
            
                }).Start();
            }
            
            // Activate task save
            if (secondsPassed % intervalSave == 0)
            {
                new Task(() => {
            
                    Trace.WriteLine(DateTime.UtcNow.ToString("o") + " -> Executing Save interval set to: " + intervalSave);
            
                }).Start();
            }
        }


    }
}
