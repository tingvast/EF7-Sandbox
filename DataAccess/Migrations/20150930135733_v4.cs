using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace DataAccess.Migrations
{
    public partial class v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Name", table: "Follower");
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Follower",
                isNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "FirstName", table: "Follower");
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Follower",
                isNullable: true);
        }
    }
}
