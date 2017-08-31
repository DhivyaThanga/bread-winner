using System;
using System.Threading;

namespace PoorManWork
{
    public class ScheduledProducer : PoorManAbstractProducer
    {
        private readonly TimeSpan _timespan;
        private readonly Func<CancellationToken, IPoorManWorkItem[]> _workFactoryMethod;

        public ScheduledProducer(TimeSpan timespan,
            Func<CancellationToken, IPoorManWorkItem[]> workFactoryMethod)
        {
            _timespan = timespan;
            _workFactoryMethod = workFactoryMethod;
        }

        protected override void Startup(
            Action<IPoorManWorkItem[], CancellationToken> addWork, CancellationToken cancellationToken)
        {
            QueueWork(addWork, cancellationToken);
        }

        protected override void QueueWork(
            Action<IPoorManWorkItem[], CancellationToken> addWork, 
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

        protected override bool WaitForWork(CancellationToken cancellationToken)
        {
            return cancellationToken.WaitHandle.WaitOne(_timespan);
        }
    }
}