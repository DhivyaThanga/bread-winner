using System.Collections.Concurrent;
using System.Threading;

namespace Api.ProductFactorLoader
{
    public interface IPoorManConsumer
    {
        void Start<T>(BlockingCollection<T> workQueue, CancellationToken cancellationToken) where T : IPoorManWorkItem;
    }
}