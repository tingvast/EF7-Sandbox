using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using DataAccess;

namespace DataAccessMigrations
{
    [ContextType(typeof(EF7BloggContext))]
    partial class vv1
    {
        public override string Id
        {
            get { return "20150711095946_vv1"; }
        }

        public override string ProductVersion
        {
            get { return "7.0.0-beta6-13735"; }
        }

        public override void BuildTargetModel(ModelBuilder builder)
        {
            builder
                .Annotation("ProductVersion", "7.0.0-vsbuild")
                .Annotation("SqlServer:ValueGenerationStrategy", "IdentityColumn");

            builder.Entity("Core.Meeting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Location");

                    b.Property<string>("Location1");

                    b.Key("Id");
                });

            builder.Entity("Core.PreRegistration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("MeetingId");

                    b.Property<string>("Text");

                    b.Property<string>("Text1");

                    b.Key("Id");
                });

            builder.Entity("Core.PreRegistration", b =>
                {
                    b.Reference("Core.Meeting")
                        .InverseCollection()
                        .ForeignKey("MeetingId");
                });
        }
    }
}
