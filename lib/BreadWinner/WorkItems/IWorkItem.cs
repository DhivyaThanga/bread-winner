using System.Threading;

namespace BreadWinner
{
    public interface IWorkItem
    {
        void Do(CancellationToken cancellationToken);
    }
}