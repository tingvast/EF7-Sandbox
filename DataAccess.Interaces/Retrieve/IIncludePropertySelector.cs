using Core;

namespace DataAccess.Interaces
{
    public interface IIncludePropertySelector<TEntity> : IPropertyProjectorBuilder<TEntity> where TEntity : class, IEntity
    {
        //IIncludePropertySelector<TEntity> ThenInclude<TProperty>(params Expression<Func<TProperty, dynamic>>[] p);
    }
}