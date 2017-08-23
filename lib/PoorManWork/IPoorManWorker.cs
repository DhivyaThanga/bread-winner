using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    public interface IPoorManWorker
    {
        void Start(BlockingCollection<IPoorManWorkItem> workQueue, CancellationToken cancellationToken);

        void Stop();
    }
}