using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using Sample;

namespace Sample.Migrations
{
    [ContextType(typeof(BloggingContext))]
    partial class myfirst
    {
        public override string Id
        {
            get { return "20150630124607_myfirst"; }
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
                
                builder.Entity("Sample.Blog", b =>
                    {
                        b.Property<int>("BlogId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 0)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("Url")
                            .Annotation("OriginalValueIndex", 1);
                        b.Key("BlogId");
                    });
                
                builder.Entity("Sample.Post", b =>
                    {
                        b.Property<int>("BlogId")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("Content")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<int>("PostId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 2)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("Title")
                            .Annotation("OriginalValueIndex", 3);
                        b.Key("PostId");
                    });
                
                builder.Entity("Sample.Post", b =>
                    {
                        b.ForeignKey("Sample.Blog", "BlogId");
                    });
                
                return builder.Model;
            }
        }
    }
}
