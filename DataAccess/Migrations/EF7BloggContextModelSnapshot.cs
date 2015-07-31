using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using DataAccess;

namespace DataAccessMigrations
{
    [ContextType(typeof(EF7BloggContext))]
    partial class EF7BloggContextModelSnapshot : ModelSnapshot
    {
        public override void BuildModel(ModelBuilder builder)
        {
            builder
                .Annotation("ProductVersion", "7.0.0-beta6-13815")
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
