using System;
using System.Threading;

namespace BreadWinner
{
    public abstract class BatchWorkItem : IWorkItem
    {
        private readonly WorkBatch _workBatch;
        private readonly CancellationToken _cancellationToken;

        public string BatchId => _workBatch.BatchId;
        public string Id { get; }
        public WorkItemStatus WorkItemStatus { get; protected set; }

        protected BatchWorkItem(
            string id, WorkBatch workBatch, CancellationToken cancellationToken)
        {
            Id = id;
            _workBatch = workBatch;
            _cancellationToken = cancellationToken;
        }

        public void Do(CancellationToken cancellationToken)
        {
            DoAlways(_cancellationToken);
            WorkItemStatus = WorkItemStatus.Successful;

            if (_workBatch.WorkDone(this))
            {
                DoFinally(_cancellationToken);
            }
        }

        protected abstract void DoAlways(CancellationToken cancellationToken);

        protected abstract void DoFinally(CancellationToken cancellationToken);
    }
}