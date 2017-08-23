using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWorkManager
{
    public class PoorManProducer<T> : PoorManWorker<T> where T : IPoorManWorkItem
    {
        private readonly int _workCheckingBackoff_ms;
        private readonly Func<CancellationToken, T> _workFactoryMethod;

        public PoorManProducer (int workCheckingBackoff_ms, Func<CancellationToken, T> workFactoryMethod)
        {
            _workCheckingBackoff_ms = workCheckingBackoff_ms;
            _workFactoryMethod = workFactoryMethod;
        }

        protected override void Loop(BlockingCollection<T> workQueue, CancellationToken cancellationToken)
        {
            while (true)
            {
                GetAllAvailableWork(workQueue, cancellationToken);

                if (cancellationToken.IsCancellationRequested ||
                    cancellationToken.WaitHandle.WaitOne(_workCheckingBackoff_ms))
                {
                    break;
                }
            }
        }

        private void GetAllAvailableWork(BlockingCollection<T> workQueue,
            CancellationToken cancellationToken)
        {
            while (true)
            {
                var workItem = _workFactoryMethod(cancellationToken);

                if (workItem != null && !cancellationToken.IsCancellationRequested)
                {
                    workQueue.Add(workItem, cancellationToken);
                }
                else
                {
                    break;
                }
            }
        }
    }
}