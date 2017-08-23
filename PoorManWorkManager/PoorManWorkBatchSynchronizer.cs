using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PoorManWorkManager
{
    public class PoorManWorkBatchSynchronizer
    {
        private int _workBatchSize;

        public PoorManWorkBatchSynchronizer(int workBatchSize)
        {
            if (workBatchSize < 1)
            {
                throw new ArgumentException();
            }

            _workBatchSize = workBatchSize;
        }

        public bool WorkDone()
        {
            Interlocked.Decrement(ref _workBatchSize);
            return _workBatchSize == 0;
        }
    }
}
