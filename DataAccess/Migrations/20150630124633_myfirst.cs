using System.Collections.Generic;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;

namespace DataAccess.Migrations
{
    public partial class myfirst : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.CreateSequence(
                name: "DefaultSequence",
                type: "bigint",
                startWith: 1L,
                incrementBy: 10);
            migration.CreateTable(
                name: "Meeting",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false),
                    Location = table.Column(type: "nvarchar(max)", nullable: true),
                    Location1 = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meeting", x => x.Id);
                });
            migration.CreateTable(
                name: "PreRegistration",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false),
                    MeetingId = table.Column(type: "int", nullable: true),
                    Text = table.Column(type: "nvarchar(max)", nullable: true),
                    Text1 = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreRegistration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreRegistration_Meeting_MeetingId",
                        columns: x => x.MeetingId,
                        referencedTable: "Meeting",
                        referencedColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
        
        public override void Down(MigrationBuilder migration)
        {
            migration.DropSequence("DefaultSequence");
            migration.DropTable("Meeting");
            migration.DropTable("PreRegistration");
        }
    }
}
