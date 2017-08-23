using System;
using System.Threading;
using PoorManWork;

namespace PoorManWorkManager
{
    public interface IPoorManWorkerFactory<T> where T : IPoorManWorkItem
    {
        IPoorManWorker<T> CreateConsumer();
        IPoorManWorker<T> CreateProducer(EventWaitHandle workArrived, Func<CancellationToken, T[]> workFactoryMethod);
    }
}