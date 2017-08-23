using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PoorManWorkManager
{
    public class PoorManSynchronizer
    {
        private int _workSize;

        public PoorManSynchronizer(int workSize)
        {
            if (workSize < 1)
            {
                throw new ArgumentException();
            }

            _workSize = workSize;
        }

        public bool WorkDone()
        {
            Interlocked.Decrement(ref _workSize);
            return _workSize == 0;
        }
    }
}
