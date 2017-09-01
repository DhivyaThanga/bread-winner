using System.Threading;

namespace BreadWinner
{
    public interface IWorkItem
    {
        string Id { get; }

        void Do(CancellationToken cancellationToken);

        WorkItemStatus WorkItemStatus { get; }
    }
}