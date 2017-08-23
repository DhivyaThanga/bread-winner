using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Api.PoorManWorkManager
{
    public class PoorManConsumerPool<T> : IPoorManWorker<T> where T : IPoorManWorkItem
    {
        private readonly IPoorManWorker<T>[] _consumers;
        private volatile bool _isStarted = false;

        public PoorManConsumerPool(int concurrency)
        {
            _consumers = new IPoorManWorker<T>[concurrency];

            for (var i = 0; i < _consumers.Length; i++)
            {
                _consumers[i] = new PoorManConsumer<T>();
            }
        }

        public void Start(BlockingCollection<T> workQueue, CancellationToken cancellationToken)
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
                throw new ApplicationException("Consumers already started");
            }
        }

        public void Stop()
        {
            if (!_isStarted) return;

            foreach (var consumer in _consumers)
            {
                consumer.Stop();
            }

            _isStarted = false;
        }
    }
}