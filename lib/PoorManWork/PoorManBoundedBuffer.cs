using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    public class PoorManBoundedBuffer : IPoorManManager
    {
        private readonly PoorManWorkerPool _workerPool;
        private readonly BlockingCollection<IPoorManWorkItem> _workQueue;

        public PoorManBoundedBuffer()
        {
            _workerPool = new PoorManWorkerPool();
            _workQueue = new BlockingCollection<IPoorManWorkItem>();
        }

        public void AddProducer(
            EventWaitHandle workArrived,
            Func<CancellationToken, IPoorManWorkItem[]> workFactoryMethod)
        {
            _workerPool.Add(
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
                _workerPool.Add(new PoorManConsumer(TakeWork));
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

        public void Start(CancellationToken cancellationToken)
        {
            _workerPool.Start(cancellationToken);
        }
    }
}