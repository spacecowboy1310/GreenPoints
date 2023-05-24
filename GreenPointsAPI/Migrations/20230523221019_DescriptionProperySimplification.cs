using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenPointsAPI.Migrations
{
    /// <inheritdoc />
    public partial class DescriptionProperySimplification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DescriptionProperties_GreenPoints_GreenPointId",
                table: "DescriptionProperties");

            migrationBuilder.DropTable(
                name: "EditDescriptionProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DescriptionProperties",
                table: "DescriptionProperties");

            migrationBuilder.RenameTable(
                name: "DescriptionProperties",
                newName: "DescriptionProperty");

            migrationBuilder.RenameIndex(
                name: "IX_DescriptionProperties_GreenPointId",
                table: "DescriptionProperty",
                newName: "IX_DescriptionProperty_GreenPointId");

            migrationBuilder.AddColumn<int>(
                name: "EditGreenPointId",
                table: "DescriptionProperty",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DescriptionProperty",
                table: "DescriptionProperty",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionProperty_EditGreenPointId",
                table: "DescriptionProperty",
                column: "EditGreenPointId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DescriptionProperty_EditGreenPoints_EditGreenPointId",
                table: "DescriptionProperty");

            migrationBuilder.DropForeignKey(
                name: "FK_DescriptionProperty_GreenPoints_GreenPointId",
                table: "DescriptionProperty");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DescriptionProperty",
                table: "DescriptionProperty");

            migrationBuilder.DropIndex(
                name: "IX_DescriptionProperty_EditGreenPointId",
                table: "DescriptionProperty");

            migrationBuilder.DropColumn(
                name: "EditGreenPointId",
                table: "DescriptionProperty");

            migrationBuilder.RenameTable(
                name: "DescriptionProperty",
                newName: "DescriptionProperties");

            migrationBuilder.RenameIndex(
                name: "IX_DescriptionProperty_GreenPointId",
                table: "DescriptionProperties",
                newName: "IX_DescriptionProperties_GreenPointId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DescriptionProperties",
                table: "DescriptionProperties",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EditDescriptionProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CollaboratorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    EditGreenPointId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EditDescriptionProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EditDescriptionProperties_EditGreenPoints_EditGreenPointId",
                        column: x => x.EditGreenPointId,
                        principalTable: "EditGreenPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EditDescriptionProperties_Users_CollaboratorId",
                        column: x => x.CollaboratorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EditDescriptionProperties_CollaboratorId",
                table: "EditDescriptionProperties",
                column: "CollaboratorId");

            migrationBuilder.CreateIndex(
                name: "IX_EditDescriptionProperties_EditGreenPointId",
                table: "EditDescriptionProperties",
                column: "EditGreenPointId");

            migrationBuilder.AddForeignKey(
                name: "FK_DescriptionProperties_GreenPoints_GreenPointId",
                table: "DescriptionProperties",
                column: "GreenPointId",
                principalTable: "GreenPoints",
                principalColumn: "Id");
        }
    }
}
