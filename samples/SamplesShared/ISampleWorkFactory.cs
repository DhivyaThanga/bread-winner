using System.Threading;
using BreadWinner;

namespace SamplesShared
{
    public interface ISampleWorkFactory
    {
        IWorkItem[] Create(CancellationToken cancellationToken);
        IWorkItem[] Startup(CancellationToken cancellationToken);
    }
}