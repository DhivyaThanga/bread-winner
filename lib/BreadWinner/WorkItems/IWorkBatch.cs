using System.Collections.Concurrent;

namespace BreadWinner
{
    public interface IWorkBatch
    {
        string Id { get; }

        bool WorkDone(AbstractBatchedWorkItem workItem);

        object[] Results { get; }
    }
}