using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace DataAccess.Migrations
{
    public partial class v6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Text", table: "Post");
            migrationBuilder.AddColumn<string>(
                name: "BlogText",
                table: "Post",
                isNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "BlogText", table: "Post");
            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Post",
                isNullable: true);
        }
    }
}
