using System;
using System.Threading;
using PoorManWorkManager;

namespace ConsoleApp
{
    public class DummyProductFactorLoader : IDisposable
    {
        private readonly IPoorManWorkManager<DummyWorkItem> _poorManWorkManager;

        public DummyProductFactorLoader()
        {
            _poorManWorkManager = new PoorManWorkManager<DummyWorkItem>();
        }

        public void Start()
        {
            _poorManWorkManager.Start(2, 1000, CreateWorkItem);
        }

        public void Dispose()
        {
            _poorManWorkManager.Dispose();
        }

        private static DummyWorkItem CreateWorkItem(CancellationToken cancellationToken)
        {
            if (cancellationToken.WaitHandle.WaitOne(1000))
            {
                return null;
            }

            var rand = new Random();
            var workItem = new DummyWorkItem(rand.Next());
            Console.WriteLine($"Producer {Thread.CurrentThread.ManagedThreadId} has created {workItem.Id}");

            return workItem;
        }
    }
}