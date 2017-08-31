using System;
using System.Threading;

namespace PoorManWork
{
    public interface IPoorManManager : IPoorManWorker
    {
        void AddConsumers(int n);

        void AddProducer(PoorManAbstractProducer producer);
    }
}