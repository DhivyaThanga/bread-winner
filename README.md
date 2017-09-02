# Bread Winner

Poor man implementation of producer consumer paradigm in C#.

This library relies heavily on threads. TPL should be avoided when using this library, even at the cost of synchrounously waiting tasks when no synchronous API is available. 

Further details in the [justification](#Justification) section, but the general idea is that the purpose of this library is avoiding to use the Managed Thread Pool. This is epescially useful in the context of Web API.

Every worker instance uses it's own thread, therefore use with caution at your own risk.

## <a name="Justification"></a> Justification

The main reason to write this library emerged from the need of downloading multiple files from Azure Blob Storage, decoding them and loading them in a shared memory cache with a Web API application. The main caveat of the process is that all async programming from TPL cannot be involved, as there is no guarantee which thread is going to pick up the task after you await. This means that you'd be stealing threads from Web API threadpool, which should be used to serve your client's requests exclusively.

The overhead of creating threads is minimal and mainly affects memory. The consumers are, as a matter of fact, all waiting for signals, and won't be scheduled unless there is some work to do. Moreover, it's possible to set up the priority of the threads wrapped in the workers, making the approach more flexible for what regards the scheduling. There is no concrete implementation of the producer in the library itself, but one is offered in the samples. This would normally be the only element that would be polling a source, and would be scheduled. If the schedule is not a very short period, the overhead caused by context switching the producer process would be minimal. The ratio has to be though that the producer would the minimum amount of work required to create work items to be consumed by the consumers.

## Components

**A Worker Pool**

The pool is a collection of workers (classes implementing the IWorker interface). The pool will be responsible of starting all of the instances registered in it. The pool has also a property called IsAlive, which will be false when one or more of children have aborted or terminated. You can use this property to monitor the health of your pool.

**<a name="YourConcreteProducer"></a> Your Concrete Producer**

## Installing

To install, you can use the related nuget package.
```powershell
Install-Package BreadWinner -Version 0.5.0
```
## Setup
Setup is pretty easy providing that:
* you have created you own concrete producer class inheriting from abstract producer
* you have a cancellation token that will be cancelled when closing you application or when needed

More on the first point in the [your concrete producer](#YourConcreteProducer) section.

You can find an easy example below. You will need to use the WorkerFactory to create objects of this library. Creating some consumers or a pool is pretty straight forward.

```csharp

IWorkerFactory factory = new WorkerFactory();

IWorker producer = factory.CreateProducer(yourConcreteProducerFactoryMethod)

IWorker consumers = factory.CreateConsumers(numberOfConcurrentConsumers);

IWorkerPool workerPool = factory.CreatePool();
workerPool.Add(producer);
workerPool.Add(consumers);

workerPool.Start(yourCancellationToken);

```

## Configuration
