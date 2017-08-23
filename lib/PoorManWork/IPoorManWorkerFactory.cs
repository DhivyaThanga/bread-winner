using System;
using System.Threading;
using PoorManWork;

namespace PoorManWorkManager
{
    public interface IPoorManWorkerFactory
    {
        IPoorManWorker CreateConsumer();
        IPoorManWorker CreateProducer(EventWaitHandle workArrived, Func<CancellationToken, IPoorManWorkItem[]> workFactoryMethod);
    }
}