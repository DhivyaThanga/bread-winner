using System;
using System.Threading;
using BreadWinner;

namespace WebApi
{
    public class ScheduledProducer : AbstractProducer
    {
        private readonly TimeSpan _timespan;
        private readonly Func<CancellationToken, IWorkItem[]> _workFactoryMethod;

        public ScheduledProducer(TimeSpan timespan,
            Func<CancellationToken, IWorkItem[]> workFactoryMethod)
        {
            _timespan = timespan;
            _workFactoryMethod = workFactoryMethod;
        }

        protected override void Startup(
            Action<IWorkItem[], CancellationToken> addWork, CancellationToken cancellationToken)
        {
            QueueWork(addWork, cancellationToken);
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