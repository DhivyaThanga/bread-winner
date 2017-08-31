using System;
using System.Threading;

namespace PoorManWork
{
    public abstract class PoorManBatchWorkItem : IPoorManWorkItem
    {
        private readonly PoorManWorkBatch _workBatch;
        private readonly CancellationToken _cancellationToken;

        public string BatchId => _workBatch.Id;
        public string Id { get; }
        public PoorManWorkItemStatus WorkItemStatus { get; private set; }

        protected PoorManBatchWorkItem(
            string id, PoorManWorkBatch workBatch, CancellationToken cancellationToken)
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
                WorkItemStatus = PoorManWorkItemStatus.Successful;
            }
            catch (Exception exception)
            {
                WorkItemStatus = PoorManWorkItemStatus.Failed;
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