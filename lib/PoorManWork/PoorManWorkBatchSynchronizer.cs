using System;
using System.Threading;

namespace PoorManWork
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
