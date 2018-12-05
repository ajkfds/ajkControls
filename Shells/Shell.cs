using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ajkControls
{
    public class Shell : IDisposable
    {
        public delegate void ReceivedHandler(string lineString);
        public virtual event ReceivedHandler LineReceived;

        public virtual void Start()
        {
        }

        public virtual void Dispose()
        {
        }

        public virtual void Execute(string command)
        {
        }

        public virtual string GetLastLine()
        {
            return "";
        }

    }
}
