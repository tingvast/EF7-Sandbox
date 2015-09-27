using System;

namespace DataAccess.Interaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository Create();

        void Commit();
    }
}