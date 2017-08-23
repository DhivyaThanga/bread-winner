using System;
using System.Threading;

namespace PoorManWorkManager
{
    public interface IPoorManWorkFacade
    {
        void Start(CancellationToken cancellationToken);

        void Stop();
    }
}