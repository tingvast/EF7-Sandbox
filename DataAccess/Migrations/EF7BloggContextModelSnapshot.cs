using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using DataAccess;

namespace DataAccess.Migrations
{
    [ContextType(typeof(EF7BloggContext))]
    partial class EF7BloggContextModelSnapshot : ModelSnapshot
    {
        public override IModel Model
        {
            get
            {
                var builder = new BasicModelBuilder()
                    .Annotation("SqlServer:ValueGeneration", "Sequence");
                
                builder.Entity("ConsoleApplication3.Meeting", b =>
                    {
                        b.Property<int>("Id")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 0)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("Location")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<string>("Location1")
                            .Annotation("OriginalValueIndex", 2);
                        b.Key("Id");
                    });
                
                builder.Entity("ConsoleApplication3.PreRegistration", b =>
                    {
                        b.Property<int>("Id")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 0)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<int?>("MeetingId")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<string>("Text")
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<string>("Text1")
                            .Annotation("OriginalValueIndex", 3);
                        b.Key("Id");
                    });
                
                builder.Entity("ConsoleApplication3.PreRegistration", b =>
                    {
                        b.ForeignKey("ConsoleApplication3.Meeting", "MeetingId");
                    });
                
                return builder.Model;
            }
        }
    }
}
