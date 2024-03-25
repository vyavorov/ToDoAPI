using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoAPI.Migrations
{
    public partial class FamilyAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FamilyId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Families",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Families", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_FamilyId",
                table: "Users",
                column: "FamilyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Families_FamilyId",
                table: "Users",
                column: "FamilyId",
                principalTable: "Families",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Families_FamilyId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Families");

            migrationBuilder.DropIndex(
                name: "IX_Users_FamilyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FamilyId",
                table: "Users");
        }
    }
}
