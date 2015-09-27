using Microsoft.Data.Entity.Migrations;

namespace DataAccess.Migrations
{
    public partial class v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Location", table: "Blog");
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Blog",
                isNullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Blog",
                isNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Description", table: "Blog");
            migrationBuilder.DropColumn(name: "Name", table: "Blog");
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Blog",
                isNullable: true);
        }
    }
}