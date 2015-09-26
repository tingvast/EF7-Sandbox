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
            get { return "20150926165638_v1"; }
        }

        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Annotation("ProductVersion", "7.0.0-beta7-15540")
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn);

            modelBuilder.Entity("Core.Meeting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Location");

                    b.Property<string>("Location1");

                    b.Key("Id");
                });

            modelBuilder.Entity("Core.PreRegistration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("MeetingId");

                    b.Property<string>("Text");

                    b.Property<string>("Text1");

                    b.Key("Id");
                });

            modelBuilder.Entity("Core.PreRegistration2", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("MeetingId");

                    b.Property<string>("Value");

                    b.Key("Id");
                });

            modelBuilder.Entity("Core.PreRegistration", b =>
                {
                    b.Reference("Core.Meeting")
                        .InverseCollection()
                        .ForeignKey("MeetingId");
                });

            modelBuilder.Entity("Core.PreRegistration2", b =>
                {
                    b.Reference("Core.Meeting")
                        .InverseCollection()
                        .ForeignKey("MeetingId");
                });
        }
    }
}
