using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace DataAccess.Migrations
{
    public partial class v5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Follower",
                isNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "LastName", table: "Follower");
        }
    }
}
