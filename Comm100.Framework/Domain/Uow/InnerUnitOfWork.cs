using System;
using System.Collections.Generic;
using System.Text;

namespace Comm100.Framework.Domain.Uow
{
    public class InnerUnitOfWork : IUnitOfWork
    {

        public int _siteId;

        //public TransactionOptions TransactionOptions => throw new NotImplementedException();

        public event EventHandler Disposed;

        public void Complete()
        {

        }

        public void Dispose()
        {

        }
    }
}
