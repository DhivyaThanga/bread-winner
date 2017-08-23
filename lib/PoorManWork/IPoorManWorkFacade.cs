using System.Threading;

namespace PoorManWork
{
    public interface IPoorManWorkFacade
    {
        void Start(CancellationToken cancellationToken);

        void Stop();
    }
}