using Microsoft.Data.Entity.Migrations;

namespace DataAccess.Migrations
{
    public partial class v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_TrackBack_Post_PostUrl", table: "TrackBack");
            migrationBuilder.DropUniqueConstraint(name: "AK_Post_Url", table: "Post");
            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "TrackBack",
                isNullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_TrackBack_Post_PostId",
                table: "TrackBack",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_TrackBack_Post_PostId", table: "TrackBack");
            migrationBuilder.DropColumn(name: "PostId", table: "TrackBack");
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Post_Url",
                table: "Post",
                column: "Url");
            migrationBuilder.AddForeignKey(
                name: "FK_TrackBack_Post_PostUrl",
                table: "TrackBack",
                column: "PostUrl",
                principalTable: "Post",
                principalColumn: "Url");
        }
    }
}