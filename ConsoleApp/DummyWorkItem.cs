using System;
using System.Threading;
using PoorManWorkManager;

namespace ConsoleApp
{
    public class DummyWorkItem : IPoorManWorkItem
    {
        public int Id { get; }

        public DummyWorkItem(int id)
        {
            Id = id;
        }

        public void Do(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} consuming {Id}");
        }
    }
}