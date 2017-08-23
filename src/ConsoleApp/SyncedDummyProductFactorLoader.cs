using System;
using System.Threading;
using System.Threading.Tasks;
using PoorManWork;
using PoorManWorkManager;

namespace ConsoleApp
{
    public class SyncedDummyProductFactorLoader : IDisposable
    {
        private readonly IPoorManWorkFacade _poorManWorkFacade;
        private static int _count;
        private readonly Task _workEmitter;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public SyncedDummyProductFactorLoader()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var are = new AutoResetEvent(false);
            _workEmitter = DummyPulser.Create(
                are, () => Interlocked.Exchange(ref _count, 0), _cancellationTokenSource.Token);

            var factory = new PoorManWorkerFactory<SyncedDummyWorkItem>();

            var producer = factory.CreateProducer(are, WorkBatchFactoryMethod);
            var consumers = factory.CreateConsumerArray(2);

            _poorManWorkFacade = new PoorManWorkFacade<SyncedDummyWorkItem>(producer, consumers);
        }

        public void Start()
        {
            _workEmitter.Start();
            _poorManWorkFacade.Start(_cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();

            _workEmitter.Wait();
        }

        private static SyncedDummyWorkItem[] WorkBatchFactoryMethod(CancellationToken cancellationToken)
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
            var synchronizer = new PoorManWorkBatchSynchronizer(3);
            var workItems = new [] {
                new SyncedDummyWorkItem(rand.Next(), synchronizer, cancellationToken),
                new SyncedDummyWorkItem(rand.Next(), synchronizer, cancellationToken),
                new SyncedDummyWorkItem(rand.Next(), synchronizer, cancellationToken)
            };
            Console.WriteLine(
                $"Producer {Thread.CurrentThread.ManagedThreadId} has created " +
                $"{workItems[0].Id}, {workItems[1].Id}, {workItems[2].Id}");

            return workItems;
        }
    }
}