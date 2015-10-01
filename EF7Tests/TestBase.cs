using Ploeh.AutoFixture;

namespace EF7Tests
{
    public class TestBase
    {
        protected Fixture Fixture;

        public TestBase()
        {
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            Fixture = new Fixture();

            Fixture.Register(() => TestDataBuilders.BuildAnyBlogWithRelations(Fixture));
            Fixture.Register(() => TestDataBuilders.BuildAnyFollower(Fixture));
            Fixture.Register(() => TestDataBuilders.BuildAnyPost(Fixture));
        }
    }
}