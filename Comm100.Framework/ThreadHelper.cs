using System;
using System.Threading;

namespace Comm100.Framework
{
    public class ThreadStartOnce
    {
        private bool _started;
        private Thread _thread;

        public ThreadStartOnce(Thread thread)
        {
            this._thread = thread;
        }

        public bool Start()
        {
            lock (this)
            {
                if (this._started) return false;

                this._thread.Start();
                this._started = true;
                return true;
            }
        }
    }
}