using System;
using System.Diagnostics;

namespace MonkeyOthello.Core
{
    public class Clock
    {
        private Stopwatch watch = new Stopwatch();

        public TimeSpan Elapsed
        {
            get { return watch.Elapsed; }
        }

        public double TotalSeconds
        {
            get
            {
                return this.Elapsed.TotalSeconds;
            }
        }

        public double TotalMilliseconds
        {
            get
            {
                return this.Elapsed.TotalMilliseconds;
            }
        }

        public Clock()
        {
            Reset();
        }

        public void Reset()
        {
            watch.Reset();
        }

        public void Start()
        {
            watch.Start();
        }

        public void Stop()
        {
            watch.Stop();
        }

        public override string ToString()
        {
            return String.Format("{0:f2} s", this.Elapsed);
        }
    }
}
