using Core;

namespace DataAccess.Interaces
{
    public interface IIncludeSelectPropertySelector<TEntity> : ISelectPropertyBuilder<TEntity> where TEntity : class, IEntity
    {
        //IIncludePropertySelector<TEntity> ThenInclude<TProperty>(params Expression<Func<TProperty, dynamic>>[] p);
    }
}