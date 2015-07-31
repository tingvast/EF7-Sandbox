using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Migrations.Builders;
using Microsoft.Data.Entity.Migrations.Operations;

namespace DataAccessMigrations
{
    public partial class v1 : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.CreateTable(
                name: "Meeting",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", "IdentityColumn"),
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
                    Id = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", "IdentityColumn"),
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
                        referencedColumn: "Id");
                });
        }

        public override void Down(MigrationBuilder migration)
        {
            migration.DropTable("PreRegistration");
            migration.DropTable("Meeting");
        }
    }
}
