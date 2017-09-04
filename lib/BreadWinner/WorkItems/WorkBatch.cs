using System;
using System.Collections.Concurrent;
using System.Threading;

namespace BreadWinner
{
    public class WorkBatch
    {
        private int _workBatchSize;
        public string BatchId { get; }

        public ConcurrentBag<IWorkItem> CompletedWorkItems { get; }

        public WorkBatch(int workBatchSize, string batchId = null)
        {
            if (workBatchSize < 1)
            {
                throw new ArgumentException();
            }

            _workBatchSize = workBatchSize;
            CompletedWorkItems = new ConcurrentBag<IWorkItem>();

            BatchId = batchId ?? Guid.NewGuid().ToString();
        }

        public bool WorkDone(IWorkItem workItem)
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
