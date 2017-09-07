using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace BreadWinner
{
    public class WorkerFactory : IWorkerFactory
    {
        private readonly BlockingCollection<IWorkItem> _workQueue;

        public WorkerFactory()
        {
            _workQueue = new BlockingCollection<IWorkItem>();
        }

        public IWorker CreateProducer(
            Func<AbstractProducer> factoryMethod)
        {
            var producer = factoryMethod();
            producer.AddWork = AddWork;

            return producer;
        }

        public IWorkerPool CreatePool()
        {
            return new WorkerPool();
        }

        public IWorker CreateScheduledJob(
            TimeSpan interval,
            Action<CancellationToken> workItem, 
            Action<CancellationToken> startupAction = null)
        {
            return new ScheduledJob(interval, workItem, startupAction);
        }

        public IWorker[] CreateConsumers(int n)
        {
            var consumers = new IWorker[n];
            for (var i = 0; i < n; i++)
            {
                consumers[i] = new Consumer(TakeWork);
            }

            return consumers;
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