using Microsoft.EntityFrameworkCore.Migrations;

namespace CarImagesWeb.Migrations
{
    public partial class RoleTagMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleTagMappings_AspNetRoles_UserRoleId1",
                table: "UserRoleTagMappings");

            migrationBuilder.DropIndex(
                name: "IX_UserRoleTagMappings_UserRoleId1",
                table: "UserRoleTagMappings");

            migrationBuilder.DropColumn(
                name: "UserRoleId1",
                table: "UserRoleTagMappings");

            migrationBuilder.AlterColumn<string>(
                name: "UserRoleId",
                table: "UserRoleTagMappings",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleTagMappings_AspNetRoles_UserRoleId",
                table: "UserRoleTagMappings",
                column: "UserRoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleTagMappings_AspNetRoles_UserRoleId",
                table: "UserRoleTagMappings");

            migrationBuilder.AlterColumn<int>(
                name: "UserRoleId",
                table: "UserRoleTagMappings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "UserRoleId1",
                table: "UserRoleTagMappings",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleTagMappings_UserRoleId1",
                table: "UserRoleTagMappings",
                column: "UserRoleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleTagMappings_AspNetRoles_UserRoleId1",
                table: "UserRoleTagMappings",
                column: "UserRoleId1",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
