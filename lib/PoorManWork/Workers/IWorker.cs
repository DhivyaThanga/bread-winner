using System.Threading;

namespace PoorManWork
{
    public interface IWorker
    {
        void Start(CancellationToken cancellationToken);

        bool IsAlive { get; }
    }
}