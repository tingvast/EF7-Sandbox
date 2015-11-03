using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using DataAccess;

namespace DataAccess.Migrations
{
    [DbContext(typeof(EF7BloggContext))]
    [Migration("20151103185532_v1")]
    partial class v1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Annotation("ProductVersion", "7.0.0-beta8-15964")
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Core.Blog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Core.Follower", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BlogId");

                    b.Property<string>("Name");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Core.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("BlogId");

                    b.Property<string>("Date");

                    b.Property<string>("Text");

                    b.Property<string>("Url");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Core.TrackBack", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<string>("PostUrl");

                    b.Property<string>("ReferrerUrl");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Core.Follower", b =>
                {
                    b.HasOne("Core.Blog")
                        .WithMany()
                        .ForeignKey("BlogId");
                });

            modelBuilder.Entity("Core.Post", b =>
                {
                    b.HasOne("Core.Blog")
                        .WithMany()
                        .ForeignKey("BlogId");
                });

            modelBuilder.Entity("Core.TrackBack", b =>
                {
                    b.HasOne("Core.Post")
                        .WithMany()
                        .ForeignKey("PostUrl")
                        .PrincipalKey("Url");
                });
        }
    }
}
