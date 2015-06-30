using ConsoleApplication3;
using EF7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interaces
{
    public interface IRepository
    {

        T Create<T>(T entity) where T : class, IEntity;

        T CreateGraph<T>(T entityWithRelations) where T : class, IEntity;

        T Retrieve<T>(int id) where T : class, IEntity;

        T Retrieve<T, TResult>(int id, Expression<Func<T, TResult>> selectedProperties) where T : class, IEntity;

        Meeting Retrieve(int j, int id);

        T RetrieveReadonly<T, TResult>(int id, Func<T, TResult> selectedProperties) where T : class, IEntity;

        T Update<T>(T entity) where T : class, IEntity;

        T Update<T, TResult>(T entity, Expression<Func<T, TResult>> selectedProperties) where T : class, IEntity;

        T UpdateGraph<T>(T entityWithRelations) where T : class, IEntity;

        void Delete<T>(T entity) where T : class, IEntity;


    }
}
