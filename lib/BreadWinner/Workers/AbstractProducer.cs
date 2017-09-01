using System;
using System.Threading;

namespace BreadWinner
{
    public abstract class AbstractProducer : Worker
    {
        internal Action<IWorkItem[], CancellationToken> AddWork { get; set; }

        protected AbstractProducer()
        {
            StartupAction = cancellatonToken =>
            {
                Startup(AddWork, cancellatonToken);
            };
        }

        protected sealed override void Loop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (WaitForWorkOrCancellation(cancellationToken))
                {
                    break;
                }

                QueueWork(AddWork, cancellationToken);
            }
        }

        protected abstract void Startup(
            Action<IWorkItem[], CancellationToken> addWork,
            CancellationToken cancellationToken);

        protected abstract void QueueWork(
            Action<IWorkItem[], CancellationToken> addWork,
            CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>true if cancellation was requested</returns>
        protected abstract bool WaitForWorkOrCancellation(CancellationToken cancellationToken);
    }
}