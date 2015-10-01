using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.SqlServer.Metadata;

namespace DataAccess.Migrations
{
    [DbContext(typeof(EF7BloggContext))]
    partial class EF7BloggContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Annotation("ProductVersion", "7.0.0-beta7-15540")
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn);

            modelBuilder.Entity("Core.Blog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Key("Id");
                });

            modelBuilder.Entity("Core.Follower", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BlogId");

                    b.Property<string>("Name");

                    b.Key("Id");
                });

            modelBuilder.Entity("Core.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn);

                    b.Property<int?>("BlogId");

                    b.Property<string>("Date");

                    b.Property<string>("Text");

                    b.Property<string>("Url");

                    b.Key("Id");
                });

            modelBuilder.Entity("Core.TrackBack", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<string>("PostUrl");

                    b.Property<string>("ReferrerUrl");

                    b.Key("Id");
                });

            modelBuilder.Entity("Core.Follower", b =>
                {
                    b.Reference("Core.Blog")
                        .InverseCollection()
                        .ForeignKey("BlogId");
                });

            modelBuilder.Entity("Core.Post", b =>
                {
                    b.Reference("Core.Blog")
                        .InverseCollection()
                        .ForeignKey("BlogId");
                });

            modelBuilder.Entity("Core.TrackBack", b =>
                {
                    b.Reference("Core.Post")
                        .InverseCollection()
                        .ForeignKey("PostUrl")
                        .PrincipalKey("Url");
                });
        }
    }
}