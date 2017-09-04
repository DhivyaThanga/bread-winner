using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BreadWinner;

namespace SamplesShared.DummyExample
{
    public class DummyWorkFactory : ISampleWorkFactory
    {
        private readonly WorkAvailableRepo _workAvailableRepo;

        public DummyWorkFactory(WorkAvailableRepo workAvailableRepo)
        {
            _workAvailableRepo = workAvailableRepo;
        }

        public IWorkItem[] Startup(CancellationToken cancellationToken)
        {
            Debug.WriteLine("Producer startup");
            return GetWorkItems(cancellationToken);
        }

        public IWorkItem[] Create(CancellationToken cancellationToken)
        {
            if (!_workAvailableRepo.IsWorkAvailable())
            {
                return null;
            }

            return GetWorkItems(cancellationToken);
        }

        private static IWorkItem[] GetWorkItems(CancellationToken cancellationToken)
        {
            var rand = new Random();
            var synchronizer = new WorkBatch(3);
            var workItems = new[]
            {
                new BatchDummyWorkItem(rand.Next(), synchronizer, cancellationToken),
                new BatchDummyWorkItem(rand.Next(), synchronizer, cancellationToken),
                new BatchDummyWorkItem(rand.Next(), synchronizer, cancellationToken)
            };

            Debug.WriteLine(
                $"Producer {Thread.CurrentThread.ManagedThreadId} has created " +
                $"{workItems[0].Id}, {workItems[1].Id}, {workItems[2].Id}");

            return workItems.Cast<IWorkItem>().ToArray();
        }
    }
}