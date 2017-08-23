using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    public interface IPoorManWorker
    {
        void Start(CancellationToken cancellationToken);

        void Stop();
    }
}