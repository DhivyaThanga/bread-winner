using System;
using System.Threading;

namespace PoorManWork
{
    public interface IPoorManWorkItem
    {
        string Id { get; }

        void Do(CancellationToken cancellationToken);

        PoorManWorkItemStatus WorkItemStatus { get; }
    }
}