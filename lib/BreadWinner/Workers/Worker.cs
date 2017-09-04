using System;
using System.Configuration;
using System.Threading;

namespace BreadWinner
{
    public abstract class Worker : IWorker
    {
        protected internal Action<CancellationToken> StartupAction { get; set; }

        protected Thread WrappedThread { get; set; }

        public bool IsAlive => WrappedThread.IsAlive;

        public void Start(
            CancellationToken cancellationToken)
        {
            if (cancellationToken != CancellationToken.None)
            {
                cancellationToken.Register(Stop);
            }

            WrappedThread = new Thread(GetThreadAction(cancellationToken))
            {
                IsBackground = true
            };

            SetThreadPriority();

            WrappedThread.Start();
        }

        protected abstract void Loop(CancellationToken cancellationToken);

        private void SetThreadPriority()
        {
            var valueString = ConfigurationManager.AppSettings[ConfigOptions.ThreadPriority];
            if (valueString == null) return;
            ThreadPriority threadPriority;
            if (Enum.TryParse(valueString, out threadPriority))
            {
                WrappedThread.Priority = threadPriority;
            }
        }

        private ThreadStart GetThreadAction(
            CancellationToken cancellationToken)
        {
            return () =>
            {
                try
                {
                    StartupAction?.Invoke(cancellationToken);

                    Loop(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Cancellation was requested
                }
            };
        }

        private void Stop()
        {
            if (!WrappedThread.Join(GetThreadWaitTime()))
            {
                WrappedThread.Abort();
            }
        }

        private static int GetThreadWaitTime()
        {
            var tp = ConfigurationManager.AppSettings[ConfigOptions.WaitXForthreads];
            if (tp == null) return 0;
            int tpInt;
            return int.TryParse(tp, out tpInt) ? tpInt : 0;
        }
    }
}