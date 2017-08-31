using System;
using System.Threading;

namespace PoorManWork
{
    public abstract class PoorManAbstractProducer : PoorManWorker
    {
        internal Action<IPoorManWorkItem[], CancellationToken> AddWork { get; set; }

        protected sealed override void Loop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (WaitForWork(cancellationToken))
                {
                    break;
                }

                QueueWork(AddWork, cancellationToken);
            }
        }

        protected abstract void QueueWork(
            Action<IPoorManWorkItem[], CancellationToken> addWork,
            CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>true if cancellation was requested</returns>
        protected abstract bool WaitForWork(CancellationToken cancellationToken);
    }
}