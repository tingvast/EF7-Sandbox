using Core;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Posts = f.Build<Post>().OmitAutoProperties().With(p => p.Date, f.Create<string>()).With(p => p.Text, f.Create<string>()).CreateMany().ToList(),
                Followers = f.Build<Follower>().OmitAutoProperties().With(p => p.Name, f.Create<string>()).CreateMany().ToList(),
            };
        }
    }
}
