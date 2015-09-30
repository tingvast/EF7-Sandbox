using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
