using System;
using System.Threading;
using BreadWinner;

namespace SamplesShared.BlobExample
{
    public class StartupReadFromBlobWorkItem : ReadFromBlobWorkItem
    {
        private readonly ManualResetEvent _started;

        public StartupReadFromBlobWorkItem(ManualResetEvent started, Action<string[]> storeResults, string blobUri, WorkBatch batch, CancellationToken cancellationToken) : base(storeResults, blobUri, batch, cancellationToken)
        {
            _started = started;
        }

        protected override void DoFinally(CancellationToken cancellationToken)
        {
            base.DoFinally(cancellationToken);

            _started.Set();
        }
    }
}