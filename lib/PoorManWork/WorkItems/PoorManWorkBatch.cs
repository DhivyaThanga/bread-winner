using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    public class PoorManWorkBatch
    {
        private int _workBatchSize;
        public string Id { get; }

        public ConcurrentBag<IPoorManWorkItem> CompletedWorkItems { get; }

        public PoorManWorkBatch(int workBatchSize, string batchId = null)
        {
            if (workBatchSize < 1)
            {
                throw new ArgumentException();
            }

            _workBatchSize = workBatchSize;
            CompletedWorkItems = new ConcurrentBag<IPoorManWorkItem>();

            Id = batchId ?? Guid.NewGuid().ToString();
        }

        public bool WorkDone(IPoorManWorkItem workItem)
        {
            if (_workBatchSize <= 0)
            {
                throw new ApplicationException("Batch completed");
            }

            Interlocked.Decrement(ref _workBatchSize);
            CompletedWorkItems.Add(workItem);

            return _workBatchSize == 0;
        }
    }
}
