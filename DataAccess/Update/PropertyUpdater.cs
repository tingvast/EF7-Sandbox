using DataAccess.Interaces;

namespace DataAccess
{
    public class PropertyUpdater<T> : IPropertyUpdater<T>
    {
        public IProjections AllProjections { get; set; }
    }
}