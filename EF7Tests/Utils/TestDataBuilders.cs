using Core;
using Ploeh.AutoFixture;
using System;
using System.Linq;

namespace EF7Tests
{
    internal static class TestDataBuilders
    {
        internal static Blog BuildAnyBlogWithRelations(Fixture f)
        {
            var blog = new Blog()
            {
                Author = f.Create<string>(),
                Description = f.Create<string>(),
                Name = f.Create<string>(),

                Posts = f.Build<Post>()
                    .OmitAutoProperties()
                    .With(p => p.Date, f.Create<string>())
                    .With(p => p.Text, f.Create<string>())
                    .CreateMany()
                    .ToList(),

                Followers = f.Build<Follower>()
                    .OmitAutoProperties()
                    .With(p => p.Name, f.Create<string>())
                    .CreateMany()
                    .ToList(),
            };

            // The Url must be unique, since its an alternate key on the post
            foreach (var p in blog.Posts)
            {
                p.Url = Guid.NewGuid().ToString();
            }

            return blog;
        }

        internal static Post BuildAnyPost(Fixture fixture)
        {
            return fixture.Build<Post>()
                .OmitAutoProperties()
                .With(p => p.Text, fixture.Create<string>())
                .With(p => p.Url, Guid.NewGuid().ToString())
                .With(p => p.Date, fixture.Create<string>())
                .Create();
        }

        internal static Follower BuildAnyFollower(Fixture f)
        {
            return new Follower()
            {
                Name = f.Create<string>()
            };
        }
    }
}