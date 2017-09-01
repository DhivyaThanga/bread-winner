using System;
using System.Threading;
using BreadWinner;

namespace SamplesShared
{
    public class ScheduledProducer : AbstractProducer
    {
        private readonly TimeSpan _timespan;
        private readonly Func<CancellationToken, IWorkItem[]> _workFactoryMethod;
        private readonly Func<CancellationToken, IWorkItem[]> _startupMethod;

        public ScheduledProducer(TimeSpan timespan,
            Func<CancellationToken, IWorkItem[]> workFactoryMethod,
            Func<CancellationToken, IWorkItem[]> startupMethod = null)
        {
            _timespan = timespan;
            _workFactoryMethod = workFactoryMethod;
            _startupMethod = startupMethod;
        }

        protected override void Startup(
            Action<IWorkItem[], CancellationToken> addWork, CancellationToken cancellationToken)
        {
            var workBatch = _startupMethod?.Invoke(cancellationToken);

            if (workBatch == null || cancellationToken.IsCancellationRequested)
            {
                return;
            }

            addWork(workBatch, cancellationToken);
        }

        protected override void QueueWork(
            Action<IWorkItem[], CancellationToken> addWork, 
            CancellationToken cancellationToken)
        {
            Console.WriteLine("Producer running...");

            while (true)
            {
                var workBatch = _workFactoryMethod(cancellationToken);

                if (workBatch == null || cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                addWork(workBatch, cancellationToken);
            }
        }

        protected override bool WaitForWorkOrCancellation(CancellationToken cancellationToken)
        {
            return cancellationToken.WaitHandle.WaitOne(_timespan);
        }
    }
}