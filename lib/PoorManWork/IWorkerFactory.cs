using System;
using System.Threading;

namespace PoorManWork
{
    public interface IWorkerFactory
    {
        IPoorManWorker CreateScheduledJob(
            TimeSpan interval, 
            Action<CancellationToken> workItem,
            Action<CancellationToken> startupAction = null);

        IPoorManWorker[] CreateConsumers(int n);

        IPoorManWorker CreateProducer(Func<PoorManAbstractProducer> factoryMethod);
    }
}