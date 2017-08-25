using System;
using System.Threading;

namespace PoorManWork
{
    public abstract class PoorManSyncedWorkItem : IPoorManWorkItem
    {
        private readonly PoorManWorkBatchSynchronizer _workBatchSynchronizer;
        private readonly CancellationToken _cancellationToken;

        public string Id { get; }
        public PoorManWorkItemStatus WorkItemStatus { get; private set; }

        protected PoorManSyncedWorkItem(
            string id, PoorManWorkBatchSynchronizer workBatchSynchronizer, CancellationToken cancellationToken)
        {
            Id = id;
            _workBatchSynchronizer = workBatchSynchronizer;
            _cancellationToken = cancellationToken;
        }

        public void Do(CancellationToken cancellationToken)
        {
            try
            {
                DoAlways(_cancellationToken);
                WorkItemStatus = PoorManWorkItemStatus.Completed;
            }
            catch (Exception exception)
            {
                WorkItemStatus = PoorManWorkItemStatus.Failed;
                DoAlwaysErrorCallback(exception, cancellationToken);
            }

            if (_workBatchSynchronizer.WorkDone(this))
            {
                DoFinally(_cancellationToken);
            }
        }

        protected abstract void DoAlways(CancellationToken cancellationToken);

        protected abstract void DoAlwaysErrorCallback(Exception exception, CancellationToken cancellationToken);

        protected abstract void DoFinally(CancellationToken cancellationToken);
    }
}