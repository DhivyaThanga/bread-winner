using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    internal abstract class PoorManWorker : IPoorManWorker
    {
        protected Thread WrappedThread { get; set; }

        public void Start(BlockingCollection<IPoorManWorkItem> workQueue, CancellationToken cancellationToken)
        {
            WrappedThread = new Thread(GetThreadAction(workQueue, cancellationToken))
            {
                IsBackground = true
            };

            WrappedThread.Start();
        }

        private ThreadStart GetThreadAction(
            BlockingCollection<IPoorManWorkItem> workQueue, CancellationToken cancellationToken)
        {
            return () =>
            {
                try
                {
                    Loop(workQueue, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Cancellation was requested
                }
            };
        }

        protected abstract void Loop(
            BlockingCollection<IPoorManWorkItem> workQueue, CancellationToken cancellationToken);

        public void Stop()
        {
            if (!WrappedThread.Join(1000))
            {
                WrappedThread.Abort();
            }
        }
    }
}