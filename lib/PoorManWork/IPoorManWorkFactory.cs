using System.Threading;

namespace PoorManWork
{
    public interface IPoorManWorkFactory
    {
        IPoorManWorkItem[] Create(CancellationToken cancellationToken);
    }
}
