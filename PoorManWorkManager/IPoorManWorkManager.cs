using System;
using System.Threading;

namespace PoorManWorkManager
{
    public interface IPoorManWorkManager<in T> : IDisposable where T : IPoorManWorkItem
    {
        void Start(int concurrency, int workCheckingBackoff_ms, Func<CancellationToken, T> workFactoryMethod);
    }
}