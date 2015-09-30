using Core;
using Ploeh.AutoFixture;
using System.Linq;

namespace EF7Tests
{
    internal static class TestDataBuilders
    {
        internal static Blog BuildAnyBlog(Fixture f)
        {
            return new Blog()
            {
                Author = f.Create<string>(),
                Description = f.Create<string>(),
                Name = f.Create<string>(),
                Posts = f.Build<Post>().OmitAutoProperties().With(p => p.Date, f.Create<string>()).With(p => p.BlogText, f.Create<string>()).CreateMany().ToList(),
                Followers = f.Build<Follower>().OmitAutoProperties().With(p => p.FirstName, f.Create<string>()).CreateMany().ToList(),
            };
        }

        internal static Follower BuildAnyFollower(Fixture f)
        {
            return new Follower()
            {
                FirstName = f.Create<string>()
            };
        }
    }
}