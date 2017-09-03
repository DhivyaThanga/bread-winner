# Bread Winner

Poor man implementation of producer consumer paradigm in C#.

This library relies heavily on threads. TPL should be avoided when using this library, even at the cost of synchrounously waiting tasks when no synchronous API is available. 

Further details in the [justification section](#Justification), but the general idea is that the purpose of this library is avoiding to use the Managed Thread Pool. This is epescially useful in the context of Web API.

Every worker instance uses it's own thread, therefore use with caution at your own risk.

## <a name="Justification"></a> Justification

The main reason to write this library emerged from the need of downloading multiple files from Azure Blob Storage, decoding them and loading them in a shared memory cache with a Web API application. The main caveat of the process is that all async programming from TPL cannot be involved, as there is no guarantee which thread is going to pick up the task after you await. This means that you'd be stealing threads from Web API threadpool, which should be used to serve your client's requests exclusively.

The overhead of creating threads is minimal and mainly affects memory. The consumers are, as a matter of fact, all waiting for signals, and won't be scheduled unless there is some work to do. Moreover, it's possible to set up the priority of the threads wrapped in the workers, making the approach more flexible for what regards the scheduling. There is no concrete implementation of the producer in the library itself, but one is offered in the samples. This would normally be the only element that would be polling a source, and would be scheduled. If the schedule is not a very short period, the overhead caused by context switching the producer process would be minimal. The ratio has to be though that the producer would the minimum amount of work required to create work items to be consumed by the consumers.

## Components

**Your Concrete Work Items**

The first thing you will need is to create a Work Item that will have implement the IWorkItem interface. They can be viewed as unit of work to do. Work items will be queued by the producer and consumed by the consumers (more on this later). 

The framework also provide an abtract class that introduces the concept of a batch. You can inherit this class if you have work that can be initially split and run in parallel but needs to be some degree of synchronization at the end.

**<a name="YourConcreteProducer"></a> Your Concrete Producer**

A concrete producer is implementing the AbstractProducer class. It will have to implement three methods, Startup, QueueWork and WaitForWorkOrCancellation. Startup method is run once, when your producer object gets instantiated, in the thread that is instantiating the object. When Startup method returns, a new thread is created, which will continuosly run the cycle WaitForWorkOrCancellation and then QueueWork.

Both the Startup method and the QueueWork method take a function to queue the work that is passed by the framework and you don't have to worry about. The function is called AddWork, and takes as parameters a cancellation token, and, more importantly, an array of IWorkItems.

```csharp
public class ScheduledProducer : AbstractProducer
{
    private readonly TimeSpan _timespan;
    private readonly Func<CancellationToken, IWorkItem[]> _workFactoryMethod;
    private readonly Func<CancellationToken, IWorkItem[]> _startupMethod;

    public ScheduledProducer(TimeSpan timespan,
        Func<CancellationToken, IWorkItem[]> workFactoryMethod,
        Func<CancellationToken, IWorkItem[]> startupMethod = null)
    {
        _timespan = timespan;
        _workFactoryMethod = workFactoryMethod;
        _startupMethod = startupMethod;
    }

    protected override void Startup(
        Action<IWorkItem[], CancellationToken> addWork, CancellationToken cancellationToken)
    {
        var workBatch = _startupMethod?.Invoke(cancellationToken);

        if (workBatch == null || cancellationToken.IsCancellationRequested)
        {
            return;
        }

        addWork(workBatch, cancellationToken);
    }

    protected override void QueueWork(
        Action<IWorkItem[], CancellationToken> addWork, 
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Producer running...");

        while (true)
        {
            var workBatch = _workFactoryMethod(cancellationToken);

            if (workBatch == null || cancellationToken.IsCancellationRequested)
            {
                break;
            }

            addWork(workBatch, cancellationToken);
        }
    }

    protected override bool WaitForWorkOrCancellation(CancellationToken cancellationToken)
    {
        return cancellationToken.WaitHandle.WaitOne(_timespan);
    }
}
```

**Consumers**

**Scheduled Job**

**Worker Pool**

The pool is a collection of workers (classes implementing the IWorker interface, e.g. any producer, consumer or scheduled job). The pool will be responsible of starting all of the instances registered in it. The pool has also a property called IsAlive, which will be false when one or more of children have aborted or terminated. You can use this property to monitor the health of your pool.

**Worker Factory**

You will need to use the WorkerFactory to create objects of this library. Creating some consumers, a scheduled job or a pool is pretty straight forward. Creating a producer requires a factory method that will tell the factory how to instantiate your concrete producer.

## Installing

To install, you can use the related nuget package.
```powershell
Install-Package BreadWinner -Version 0.5.0
```
## Setup
Setup is pretty easy providing that:
* you have created you own concrete producer class inheriting from abstract producer
* you have a cancellation token that will be cancelled when closing your application or when needed. The entire lifecycle of the workers is managed through the cancellation token, so beware that when a cancellation is requested on the passed token, the pool will stop. You can configure the amount of time that will be waited before forcing the pool to stop, you can find how in the [configuration section](#Configuration)

More on the first point in the [your concrete producer section](#YourConcreteProducer).

```csharp

IWorkerFactory factory = new WorkerFactory();

IWorker producer = factory.CreateProducer(yourConcreteProducerFactoryMethod)

IWorker consumers = factory.CreateConsumers(numberOfConcurrentConsumers);

IWorkerPool workerPool = factory.CreatePool();
workerPool.Add(producer);
workerPool.Add(consumers);

workerPool.Start(yourCancellationToken);

```

## <a name="Configuration"></a>Configuration
