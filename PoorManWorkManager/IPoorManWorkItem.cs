using System.Threading;

namespace PoorManWorkManager
{
    public interface IPoorManWorkItem
    {
        void Do(CancellationToken cancellationToken);
    }
}