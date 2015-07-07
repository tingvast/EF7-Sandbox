using Core;
using DataAccess.Interaces;
using EF7;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        EF7BloggContext context;
        public UnitOfWork()
        {
            this.context = new EF7BloggContext();
            this.context.ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public IRepository Create()
        {
            return new Repository(context);
        }

        public void Commit()
        {
            context.SaveChanges();
            context.Dispose();
        }

        public void Dispose()
        {
         
        }
    }
}
