using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using PoorManWork;

namespace Api
{
    public class ReadFromBlobWorkFactory : IPoorManWorkFactory
    {
        public IPoorManWorkItem[] Create(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}