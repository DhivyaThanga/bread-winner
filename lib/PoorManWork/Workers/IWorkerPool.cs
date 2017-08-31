using System.Threading;

namespace PoorManWork
{
    public interface IWorkerPool : IWorker
    {
        void Add(params IWorker[] workers);
    }
}