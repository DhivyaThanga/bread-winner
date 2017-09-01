using System;
using System.Threading;
using BreadWinner;

namespace ConsoleApp
{
    public class DummyWorkItem : IWorkItem
    {
        public string Id { get; }

        public WorkItemStatus WorkItemStatus { get; private set; }


        public DummyWorkItem(int id)
        {
            Id = id.ToString();
            WorkItemStatus = WorkItemStatus.Scheduled;
        }

        public void Do(CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} consuming {Id}");
                cancellationToken.WaitHandle.WaitOne(2000);
                WorkItemStatus = WorkItemStatus.Successful;
            }
            catch (Exception)
            {
                WorkItemStatus = WorkItemStatus.Failed;
            }
        }
    }
}