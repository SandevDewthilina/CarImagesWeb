using Microsoft.EntityFrameworkCore.Migrations;

namespace CarImagesWeb.Migrations
{
    public partial class CountryInImageUpload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "ImageUploads",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ImageUploads_CountryId",
                table: "ImageUploads",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageUploads_Countries_CountryId",
                table: "ImageUploads",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageUploads_Countries_CountryId",
                table: "ImageUploads");

            migrationBuilder.DropIndex(
                name: "IX_ImageUploads_CountryId",
                table: "ImageUploads");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "ImageUploads");
        }
    }
}
