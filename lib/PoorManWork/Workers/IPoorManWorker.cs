using System.Threading;

namespace PoorManWork
{
    public interface IPoorManWorker
    {
        void Start(CancellationToken cancellationToken);

        bool IsAlive { get; }
    }
}