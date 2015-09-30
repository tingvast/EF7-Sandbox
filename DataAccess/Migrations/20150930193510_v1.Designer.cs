using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using DataAccess;
using Microsoft.Data.Entity.SqlServer.Metadata;

namespace DataAccess.Migrations
{
    [DbContext(typeof(EF7BloggContext))]
    partial class v1
    {
        public override string Id
        {
            get { return "20150930193510_v1"; }
        }

        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BlogId");

                    b.Property<string>("Date");

                    b.Property<string>("Text");

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
        }
    }
}
