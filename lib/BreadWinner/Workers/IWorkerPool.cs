namespace BreadWinner
{
    public interface IWorkerPool : IWorker
    {
        void Add(params IWorker[] workers);
    }
}