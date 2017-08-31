using System;
using System.Threading;

namespace PoorManWork
{
    public interface IWorkItem
    {
        string Id { get; }

        void Do(CancellationToken cancellationToken);

        WorkItemStatus WorkItemStatus { get; }
    }
}