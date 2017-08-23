# Poor Man Work or Producer Consumer in C#

Poor man implementation of producer consumer paradigm in C#.

This library relies heavily on threads and does not support async.
This is by design in order to avoid using C# thread pool.

Every worker instance uses it's own thread, therefore use with caution at your own risk.

**Installing**

To install, you can use the related nuget package.
```powershell
Install-Package PoorManWork -Version 0.1.0
```
