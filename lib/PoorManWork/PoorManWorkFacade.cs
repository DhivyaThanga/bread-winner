using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    public class PoorManWorkFacade : IPoorManWorkFacade
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IPoorManWorker _producer;
        private readonly PoorManWorkerPool _consumerPool;
        private readonly BlockingCollection<IPoorManWorkItem> _workQueue;
        private volatile bool _isStarted = false;

        public PoorManWorkFacade(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _consumerPool = new PoorManWorkerPool();
            _workQueue = new BlockingCollection<IPoorManWorkItem>();
        }

        public void AddProducer(
            EventWaitHandle workArrived,
            Func<CancellationToken, IPoorManWorkItem[]> workFactoryMethod)
        {
            _consumerPool.Add(
                new PoorManProducer(workArrived, AddWork, workFactoryMethod));
        }

        public void AddProducer(PoorManPulser pulser, Func<CancellationToken, IPoorManWorkItem[]> workFactoryMethod)
        {
            AddProducer(pulser.WorkArrived, workFactoryMethod);
        }

        public void AddConsumers(int n)
        {
            for (var i = 0; i < n; i++)
            {
                _consumerPool.Add(new PoorManConsumer(TakeWork));
            }
        }

        private void AddWork(IPoorManWorkItem[] workBatch, CancellationToken cancellationToken)
        {
            foreach (var workItem in workBatch)
            {
                _workQueue.Add(workItem, cancellationToken);
            }
        }

        private IPoorManWorkItem TakeWork(CancellationToken cancellationToken)
        {
            return _workQueue.Take(cancellationToken);
        }

        public void Start()
        {
            _consumerPool.Start(_cancellationToken);
        }

        public void Stop()
        {
            _consumerPool.Stop();
        }
    }
}