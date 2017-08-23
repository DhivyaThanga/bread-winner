using System;
using System.Runtime.InteropServices;
using System.Threading;
using PoorManWorkManager;

namespace ConsoleApp
{
    public class DummyProductFactorLoader : IDisposable
    {
        private readonly IPoorManWorkManager<DummyWorkItem> _poorManWorkManager;
        private static int _count;
        private Thread _thread;
        private CancellationTokenSource _cancellationTokenSource;

        public DummyProductFactorLoader()
        {
            _poorManWorkManager = new PoorManWorkManager<DummyWorkItem>();
        }

        public void Start()
        {
            var are = new AutoResetEvent(false);
            _cancellationTokenSource = new CancellationTokenSource();
            _thread = CreateWorkEmitter(are, _cancellationTokenSource.Token);
            _poorManWorkManager.Start(2, are, CreateWorkItem);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _poorManWorkManager.Dispose();

            if (_thread.Join(1000))
            {
                _thread.Abort();
            }
        }

        private static Thread CreateWorkEmitter(EventWaitHandle workArrived, CancellationToken cancellationToken)
        {
            var emitter = new Thread(() =>
            {
                while (true)
                {
                    Console.WriteLine("Emitter runnning...");
                    Interlocked.Exchange(ref _count, 0);
                    workArrived.Set();
                    cancellationToken.WaitHandle.WaitOne(10000);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            });

            emitter.Start();

            return emitter;
        }

        private static DummyWorkItem[] CreateWorkItem(CancellationToken cancellationToken)
        {
            if (_count > 1)
            {
                return null;
            }

            Interlocked.Increment(ref _count);

            if (cancellationToken.WaitHandle.WaitOne(1000))
            {
                return null;
            }

            var rand = new Random();
            var workItems = new [] {
                new DummyWorkItem(rand.Next()),
                new DummyWorkItem(rand.Next()),
                new DummyWorkItem(rand.Next())
            };
            Console.WriteLine(
                $"Producer {Thread.CurrentThread.ManagedThreadId} has created " +
                $"{workItems[0].Id}, {workItems[1].Id}, {workItems[2].Id}");

            return workItems;
        }
    }
}