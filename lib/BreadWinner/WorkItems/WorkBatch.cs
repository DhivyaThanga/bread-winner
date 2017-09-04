using System;
using System.Collections.Concurrent;
using System.Threading;

namespace BreadWinner
{
    public class WorkBatch
    {
        private int _workBatchSize;
        public string Id { get; }

        public ConcurrentBag<AbstractBatchedWorkItem> CompletedWorkItems { get; }

        public WorkBatch(int workBatchSize, string batchId = null)
        {
            if (workBatchSize < 1)
            {
                throw new ArgumentException();
            }

            _workBatchSize = workBatchSize;
            CompletedWorkItems = new ConcurrentBag<AbstractBatchedWorkItem>();

            Id = batchId ?? Guid.NewGuid().ToString();
        }

        public bool WorkDone(AbstractBatchedWorkItem workItem)
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
