using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.SqlServer.Metadata;

namespace DataAccess.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Post",
                isNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Post_Url",
                table: "Post",
                column: "Url");
            migrationBuilder.CreateTable(
                name: "TrackBack",
                columns: table => new
                {
                    Id = table.Column<int>(isNullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn),
                    Content = table.Column<string>(isNullable: true),
                    PostUrl = table.Column<string>(isNullable: true),
                    ReferrerUrl = table.Column<string>(isNullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackBack", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackBack_Post_PostUrl",
                        column: x => x.PostUrl,
                        principalTable: "Post",
                        principalColumn: "Url");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(name: "AK_Post_Url", table: "Post");
            migrationBuilder.DropColumn(name: "Url", table: "Post");
            migrationBuilder.DropTable("TrackBack");
        }
    }
}