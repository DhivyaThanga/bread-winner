using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Api.PoorManWorkManager
{
    public abstract class PoorManWorker<T> : IPoorManWorker<T> where T : IPoorManWorkItem
    {
        protected Thread WrappedThread { get; set; }

        public void Start(BlockingCollection<T> workQueue, CancellationToken cancellationToken)
        {
            WrappedThread = new Thread(() => Start(workQueue, cancellationToken))
            {
                IsBackground = true
            };

            WrappedThread.Start();
        }

        protected abstract void Loop(BlockingCollection<T> workQueue, CancellationToken cancellationToken);

        public void Stop()
        {
            if (!WrappedThread.Join(1000))
            {
                WrappedThread.Abort();
            }
        }
    }
}