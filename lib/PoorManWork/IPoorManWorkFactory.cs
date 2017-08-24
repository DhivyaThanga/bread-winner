using System.Threading;
using PoorManWork;

namespace PoorManWorkManager
{
    public interface IPoorManWorkFactory
    {
        IPoorManWorkItem[] Create(CancellationToken cancellationToken);
    }
}
