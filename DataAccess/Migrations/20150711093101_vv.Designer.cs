using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using DataAccess;

namespace DataAccess.Migrations
{
    [ContextType(typeof(EF7BloggContext))]
    partial class vv
    {
        public override string Id
        {
            get { return "20150711093101_vv"; }
        }
        
        public override string ProductVersion
        {
            get { return "7.0.0-beta4-12943"; }
        }
        
        public override IModel Target
        {
            get
            {
                var builder = new BasicModelBuilder()
                    .Annotation("SqlServer:ValueGeneration", "Sequence");
                
                builder.Entity("Core.Meeting", b =>
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
                
                builder.Entity("Core.PreRegistration", b =>
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
                
                builder.Entity("Core.PreRegistration", b =>
                    {
                        b.ForeignKey("Core.Meeting", "MeetingId");
                    });
                
                return builder.Model;
            }
        }
    }
}
