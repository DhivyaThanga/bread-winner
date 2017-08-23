using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    internal class PoorManProducer : PoorManWorker
    {
        private readonly EventWaitHandle _workArrived;
        private readonly Func<CancellationToken, IPoorManWorkItem[]> _workFactoryMethod;

        public PoorManProducer (
            EventWaitHandle workArrived, Func<CancellationToken, IPoorManWorkItem[]> workFactoryMethod)
        {
            _workArrived = workArrived;
            _workFactoryMethod = workFactoryMethod;
        }

        protected override void Loop(
            BlockingCollection<IPoorManWorkItem> workQueue, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (WaitForWork(cancellationToken))
                {
                    break;
                }

                GetAllAvailableWork(workQueue, cancellationToken);
            }
        }

        private bool WaitForWork(CancellationToken cancellationToken)
        {
            var eventThatSignaledIndex = WaitHandle.WaitAny(new [] { cancellationToken.WaitHandle, _workArrived });
            var wasCanceled = eventThatSignaledIndex == 0;

            return wasCanceled;
        }

        private void GetAllAvailableWork(
            BlockingCollection<IPoorManWorkItem> workQueue,
            CancellationToken cancellationToken)
        {
            while (true)
            {
                var workBatch = _workFactoryMethod(cancellationToken);

                if (workBatch == null || cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                foreach (var workItem in workBatch)
                {
                    workQueue.Add(workItem, cancellationToken);
                }
            }
        }
    }
}