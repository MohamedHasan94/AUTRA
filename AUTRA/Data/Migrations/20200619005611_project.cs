using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AUTRA.Data.Migrations
{
    public partial class project : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Designer",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModefied",
                table: "Project",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "Project",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Designer",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "LastModefied",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Project");
        }
    }
}
