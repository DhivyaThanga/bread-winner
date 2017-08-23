using System.Collections.Concurrent;
using System.Threading;

namespace Api.PoorManWorkManager
{
    public interface IPoorManWorker<T> where T : IPoorManWorkItem
    {
        void Start(BlockingCollection<T> workQueue, CancellationToken cancellationToken);

        void Stop();
    }
}