using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// Rozszezenie klasy Thread
    /// http://stackoverflow.com/questions/8123461/unable-to-inherit-from-a-thread-class-in-c-sharp
    /// </summary>
    abstract class BaseThread
    {
        private Thread _thread;

        protected BaseThread()
        {
            _thread = new Thread(new ThreadStart(this.RunThread));
        }
        public void Start() { _thread.Start(); }
        public void Join() { _thread.Join(); }
        public bool IsAlive { get { return _thread.IsAlive; } }

        public abstract void RunThread();
    }


}
