using System;
using System.Collections.Concurrent;
using System.Threading;

namespace BreadWinner
{
    public class WorkPoolBuilder
    {
        private readonly IWorkerPool _workerPool;
        private readonly BlockingCollection<IWorkItem> _workQueue;

        public WorkPoolBuilder()
        {
            _workerPool = new WorkerPool();
            _workQueue = new BlockingCollection<IWorkItem>();

        }

        public WorkPoolBuilder WithNConsumers(int n)
        {
            for (var i = 0; i < n; i++)
            {
                _workerPool.Add(new Consumer(TakeWork));
            }

            return this;
        }

        public WorkPoolBuilder WithProducer(
            Func<AbstractProducer> factoryMethod)
        {
            var producer = factoryMethod();
            producer.AddWork = AddWork;

            _workerPool.Add(producer);

            return this;
        }

        public WorkPoolBuilder WithScheduledJob(
            TimeSpan interval,
            Action<CancellationToken> workItem,
            Action<CancellationToken> startupAction = null)
        {
            _workerPool.Add(new ScheduledJob(interval, workItem, startupAction));

            return this;
        }

        public IWorkerPool Build()
        {
            return _workerPool;
        }

        private void AddWork(IWorkItem[] workItems, CancellationToken cancellationToken)
        {
            foreach (var workItem in workItems)
            {
                _workQueue.Add(workItem, cancellationToken);
            }
        }

        private IWorkItem TakeWork(CancellationToken cancellationToken)
        {
            return _workQueue.Take(cancellationToken);
        }
    }
}
