namespace DataAccess.Interaces
{
    public class UoWFactory
    {
        public static IUnitOfWork Create()
        {
            return new UnitOfWork();
        }
    }
}