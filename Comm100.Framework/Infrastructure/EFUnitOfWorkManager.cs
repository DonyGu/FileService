using Comm100.Framework.Domain.Uow;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Comm100.Framework.Infrastructure
{
    public class EFUnitOfWorkManager : IUnitOfWorkManager
    {
        private DbContext _dbContext;
        //private Tenant _tenant;
        private IUnitOfWork _outerUow;

        public EFUnitOfWorkManager(DbContext dbContext)// ITenantProvider tenantProvider
        {
            this._dbContext = dbContext;
            //this._tenant = tenantProvider.GetTenant();
        }

        public IUnitOfWork Current => _outerUow;

        public IUnitOfWork Begin()
        {
            return Begin(IsolationLevel.ReadCommitted);
        }

        public IUnitOfWork Begin(IsolationLevel isolationLevel)
        {
            ///已经存在外部工作单元,则返回一个InnerUnitOfWork
            if (_outerUow != null)
                return new InnerUnitOfWork();
            var option = new TransactionOptions
            {
                IsolationLevel = isolationLevel
            };

            var uow = new EFUnitOfWork(_dbContext);//, option, _tenant

            uow.Disposed += (sender, e) =>
            {
                _outerUow = null;
            };

            _outerUow = uow;

            return _outerUow;
        }
    }
}