using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    internal class PoorManProducer : PoorManWorker
    {
        private readonly EventWaitHandle _workArrived;
        private readonly Action<IPoorManWorkItem[], CancellationToken> _addWork;
        private readonly Func<CancellationToken, IPoorManWorkItem[]> _workFactoryMethod;

        public PoorManProducer (
            EventWaitHandle workArrived, 
            Action<IPoorManWorkItem[], CancellationToken> addWork,
            Func<CancellationToken, IPoorManWorkItem[]> workFactoryMethod)
        {
            _workArrived = workArrived;
            _addWork = addWork;
            _workFactoryMethod = workFactoryMethod;
        }

        protected override void Loop( CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (WaitForWork(cancellationToken))
                {
                    break;
                }

                GetAllAvailableWork(cancellationToken);
            }
        }

        private bool WaitForWork(CancellationToken cancellationToken)
        {
            var eventThatSignaledIndex = WaitHandle.WaitAny(new [] { cancellationToken.WaitHandle, _workArrived });
            var wasCanceled = eventThatSignaledIndex == 0;

            return wasCanceled;
        }

        private void GetAllAvailableWork(CancellationToken cancellationToken)
        {
            while (true)
            {
                var workBatch = _workFactoryMethod(cancellationToken);

                if (workBatch == null || cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                _addWork(workBatch, cancellationToken);

            }
        }
    }
}