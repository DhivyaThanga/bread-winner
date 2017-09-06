using System;
using System.Collections.Concurrent;
using System.Threading;

namespace BreadWinner
{
    public class WorkBatch : IWorkBatch
    {
        private int _workBatchSize;
        private int _status;
        private readonly ConcurrentBag<object> _results;


        public string Id { get; }

        public WorkStatus Status
        {
            get { return (WorkStatus) _status; }
            set { Interlocked.Exchange(ref _status, (int) value); }
        }

        public object[] Results => _results.ToArray();

        public WorkBatch(int workBatchSize, string batchId = null)
        {
            if (workBatchSize < 1)
            {
                throw new ArgumentException();
            }

            _workBatchSize = workBatchSize;
            _results = new ConcurrentBag<object>();

            Id = batchId ?? Guid.NewGuid().ToString();
            Status = WorkStatus.Scheduled;
        }

        public bool WorkDone(AbstractBatchedWorkItem workItem)
        {
            if (_workBatchSize <= 0)
            {
                throw new ApplicationException("Batch completed");
            }

            Interlocked.Decrement(ref _workBatchSize);
            var batchDone = _workBatchSize == 0;

            if (workItem.Status == WorkStatus.Failed)
            {
                Status = WorkStatus.Failed;
            }
            else
            {
                WorkItemWasSuccessful(workItem, batchDone);
            }

            return batchDone;
        }

        private void WorkItemWasSuccessful(AbstractBatchedWorkItem workItem, bool batchDone)
        {
            _results.Add(workItem.Result);
            if (batchDone)
            {
                Status = WorkStatus.Successful;
            }
        }
    }
}
