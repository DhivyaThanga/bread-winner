using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Api.PoorManWorkManager
{
    public class PoorManProducer<T> : PoorManWorker<T> where T : IPoorManWorkItem
    {
        private readonly int _workCheckingBackoff_ms;
        private readonly Func<T> _workFactoryMethod;

        public PoorManProducer (int workCheckingBackoff_ms, Func<T> workFactoryMethod)
        {
            _workCheckingBackoff_ms = workCheckingBackoff_ms;
            _workFactoryMethod = workFactoryMethod;
        }

        protected override void Loop(BlockingCollection<T> workQueue, CancellationToken cancellationToken)
        {
            while (true)
            {
                GetAllAvailableWork(workQueue, cancellationToken);

                if (cancellationToken.WaitHandle.WaitOne(_workCheckingBackoff_ms)) break;
            }
        }

        private void GetAllAvailableWork(BlockingCollection<T> workQueue,
            CancellationToken cancellationToken)
        {
            while (true)
            {
                var workItem = _workFactoryMethod();
                if (workItem != null)
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