using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    public class PoorManWorkBatch
    {
        private int _workBatchSize;
        public string Id { get; }

        public ConcurrentBag<IPoorManWorkItem> FailedWorkItems { get; }

        public PoorManWorkBatch(int workBatchSize, string batchId = null)
        {
            if (workBatchSize < 1)
            {
                throw new ArgumentException();
            }

            _workBatchSize = workBatchSize;
            FailedWorkItems = new ConcurrentBag<IPoorManWorkItem>();

            Id = batchId ?? Guid.NewGuid().ToString();
        }

        public bool WorkDone(IPoorManWorkItem workItem)
        {
            Interlocked.Decrement(ref _workBatchSize);
            if (workItem.WorkItemStatus == PoorManWorkItemStatus.Failed)
            {
                FailedWorkItems.Add(workItem);
            }

            return _workBatchSize == 0;
        }
    }
}
