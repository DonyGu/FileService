using System;
using System.Collections.Generic;
using System.Text;

namespace Comm100.Framework.Domain.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        event EventHandler Disposed;
        void Complete();
        //TransactionOptions TransactionOptions { get; }
    }
}
