using JobRender.Data;
using JobRenderData.Repositories;
using System;
using System.Data.Entity;

namespace JobRenderData.UnitOfWork
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly EFContext _dbContext;
        public bool  _result{ get; set; }

        public EFUnitOfWork(EFContext dbContext)
        {
            Database.SetInitializer<EFContext>(null);

            if (dbContext == null)
                throw new ArgumentNullException("dbContext null olamaz.");

            _dbContext = dbContext;
        }

        #region IUnitOfWork Members
        public IRepository<T> GetRepository<T>() where T : class
        {
            return new EFRepository<T>(_dbContext);
        }

        public bool SaveChanges()
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.SaveChanges();
                    transaction.Commit();
                    _result = true;
                }
                catch
                {
                   transaction.Rollback();
                    _result = false;
                }
                finally
                {
                    Dispose();
                }
                return _result;
            }
        }
        #endregion

        #region IDisposable Members
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }

            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
