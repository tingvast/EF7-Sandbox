namespace DataAccess.Interaces
{
    public interface IPropertyUpdater<T>
    {
        IProjections AllProjections { get; set; }
    }
}