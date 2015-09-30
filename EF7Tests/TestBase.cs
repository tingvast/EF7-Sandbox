using Ploeh.AutoFixture;

namespace EF7Tests
{
    public class TestBase
    {
        protected Fixture Fixture;

        public TestBase()
        {
            Fixture = new Fixture();

            Fixture.Register(() => TestDataBuilders.BuildAnyBlogWithRelations(Fixture));
            Fixture.Register(() => TestDataBuilders.BuildAnyFollower(Fixture));
        }
    }
}