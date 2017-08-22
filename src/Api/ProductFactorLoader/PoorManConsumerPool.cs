using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Api.ProductFactorLoader
{
    public class PoorManConsumerPool : IPoorManConsumer
    {
        private readonly IPoorManConsumer[] _consumers;
        private volatile bool _isStarted = false;

        public PoorManConsumerPool(int size)
        {
            _consumers = new IPoorManConsumer[size];

            for (var i = 0; i < _consumers.Length; i++)
            {
                _consumers[i] = new PoorManConsumer();
            }
        }

        public void Start<T>(BlockingCollection<T> workQueue, CancellationToken cancellationToken)
            where T : IPoorManWorkItem
        {
            if (!_isStarted)
            {
                _isStarted = true;
                foreach (var consumer in _consumers)
                {
                    consumer.Start(workQueue, cancellationToken);
                }
            }
            else
            {
                throw new ApplicationException($"{nameof(PoorManConsumerPool)} already started");
            }
        }
    }
}