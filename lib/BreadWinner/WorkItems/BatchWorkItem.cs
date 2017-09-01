using System;
using System.Threading;

namespace BreadWinner
{
    public abstract class BatchWorkItem : IWorkItem
    {
        private readonly WorkBatch _workBatch;
        private readonly CancellationToken _cancellationToken;

        public string BatchId => _workBatch.Id;
        public string Id { get; }
        public WorkItemStatus WorkItemStatus { get; private set; }

        protected BatchWorkItem(
            string id, WorkBatch workBatch, CancellationToken cancellationToken)
        {
            Id = id;
            _workBatch = workBatch;
            _cancellationToken = cancellationToken;
        }

        public void Do(CancellationToken cancellationToken)
        {
            try
            {
                DoAlways(_cancellationToken);
                WorkItemStatus = WorkItemStatus.Successful;
            }
            catch (Exception exception)
            {
                WorkItemStatus = WorkItemStatus.Failed;
                DoAlwaysErrorCallback(exception, cancellationToken);
            }

            if (_workBatch.WorkDone(this))
            {
                DoFinally(_cancellationToken);
            }
        }

        protected abstract void DoAlways(CancellationToken cancellationToken);

        protected abstract void DoAlwaysErrorCallback(Exception exception, CancellationToken cancellationToken);

        protected abstract void DoFinally(CancellationToken cancellationToken);
    }
}