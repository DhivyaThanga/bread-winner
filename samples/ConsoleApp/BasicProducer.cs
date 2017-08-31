using System;
using System.Threading;

namespace PoorManWork
{
    public class BasicProducer : PoorManAbstractProducer
    {
        private readonly TimeSpan _timespan;
        private Func<CancellationToken, IPoorManWorkItem[]> _workFactoryMethod;

        public BasicProducer(TimeSpan timespan)
        {
            _timespan = timespan;
        }

        protected override void QueueWork(
            Action<IPoorManWorkItem[], CancellationToken> addWork, 
            CancellationToken cancellationToken)
        {
            while (true)
            {
                var workBatch = _workFactoryMethod(cancellationToken);

                if (workBatch == null || cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        protected override bool WaitForWork(CancellationToken cancellationToken)
        {
            return cancellationToken.WaitHandle.WaitOne(_timespan);
        }
    }
}