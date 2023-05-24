using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenPointsAPI.Migrations
{
    /// <inheritdoc />
    public partial class DescriptionPropertiesCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DescriptionProperty_EditGreenPoints_EditGreenPointId",
                table: "DescriptionProperty");

            migrationBuilder.DropForeignKey(
                name: "FK_DescriptionProperty_GreenPoints_GreenPointId",
                table: "DescriptionProperty");

            migrationBuilder.AddForeignKey(
                name: "FK_DescriptionProperty_EditGreenPoints_EditGreenPointId",
                table: "DescriptionProperty",
                column: "EditGreenPointId",
                principalTable: "EditGreenPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DescriptionProperty_GreenPoints_GreenPointId",
                table: "DescriptionProperty",
                column: "GreenPointId",
                principalTable: "GreenPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DescriptionProperty_EditGreenPoints_EditGreenPointId",
                table: "DescriptionProperty");

            migrationBuilder.DropForeignKey(
                name: "FK_DescriptionProperty_GreenPoints_GreenPointId",
                table: "DescriptionProperty");

            migrationBuilder.AddForeignKey(
                name: "FK_DescriptionProperty_EditGreenPoints_EditGreenPointId",
                table: "DescriptionProperty",
                column: "EditGreenPointId",
                principalTable: "EditGreenPoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DescriptionProperty_GreenPoints_GreenPointId",
                table: "DescriptionProperty",
                column: "GreenPointId",
                principalTable: "GreenPoints",
                principalColumn: "Id");
        }
    }
}
