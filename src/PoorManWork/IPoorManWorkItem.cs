using System.Threading;

namespace PoorManWork
{
    public interface IPoorManWorkItem
    {
        void Do(CancellationToken cancellationToken);
    }
}