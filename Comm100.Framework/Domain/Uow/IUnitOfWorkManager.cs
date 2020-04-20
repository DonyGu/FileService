using System;
using System.Collections.Generic;
using System.Text;

namespace Comm100.Framework.Domain.Uow
{
    public interface IUnitOfWorkManager
    {
        IUnitOfWork Begin();
        //IUnitOfWork Begin(IsolationLevel isolationLevel);
        IUnitOfWork Current { get; }
    }
}
