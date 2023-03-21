using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenPointsAPI.Migrations
{
    /// <inheritdoc />
    public partial class EditTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DescriptionProperty_GreenPoint_GreenPointId",
                table: "DescriptionProperty");

            migrationBuilder.DropForeignKey(
                name: "FK_GreenPointUser_GreenPoint_CollaborationsId",
                table: "GreenPointUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GreenPoint",
                table: "GreenPoint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DescriptionProperty",
                table: "DescriptionProperty");

            migrationBuilder.RenameTable(
                name: "GreenPoint",
                newName: "GreenPoints");

            migrationBuilder.RenameTable(
                name: "DescriptionProperty",
                newName: "DescriptionProperties");

            migrationBuilder.RenameIndex(
                name: "IX_DescriptionProperty_GreenPointId",
                table: "DescriptionProperties",
                newName: "IX_DescriptionProperties_GreenPointId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GreenPoints",
                table: "GreenPoints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DescriptionProperties",
                table: "DescriptionProperties",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EditGreenPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Latitude = table.Column<long>(type: "INTEGER", nullable: true),
                    Longitude = table.Column<long>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    CollaboratorId = table.Column<int>(type: "INTEGER", nullable: true),
                    OriginalId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EditGreenPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EditGreenPoints_GreenPoints_OriginalId",
                        column: x => x.OriginalId,
                        principalTable: "GreenPoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EditGreenPoints_Users_CollaboratorId",
                        column: x => x.CollaboratorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EditDescriptionProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CollaboratorId = table.Column<int>(type: "INTEGER", nullable: false),
                    EditGreenPointId = table.Column<int>(type: "INTEGER", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_EditGreenPoints_CollaboratorId",
                table: "EditGreenPoints",
                column: "CollaboratorId");

            migrationBuilder.CreateIndex(
                name: "IX_EditGreenPoints_OriginalId",
                table: "EditGreenPoints",
                column: "OriginalId");

            migrationBuilder.AddForeignKey(
                name: "FK_DescriptionProperties_GreenPoints_GreenPointId",
                table: "DescriptionProperties",
                column: "GreenPointId",
                principalTable: "GreenPoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GreenPointUser_GreenPoints_CollaborationsId",
                table: "GreenPointUser",
                column: "CollaborationsId",
                principalTable: "GreenPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DescriptionProperties_GreenPoints_GreenPointId",
                table: "DescriptionProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_GreenPointUser_GreenPoints_CollaborationsId",
                table: "GreenPointUser");

            migrationBuilder.DropTable(
                name: "EditDescriptionProperties");

            migrationBuilder.DropTable(
                name: "EditGreenPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GreenPoints",
                table: "GreenPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DescriptionProperties",
                table: "DescriptionProperties");

            migrationBuilder.RenameTable(
                name: "GreenPoints",
                newName: "GreenPoint");

            migrationBuilder.RenameTable(
                name: "DescriptionProperties",
                newName: "DescriptionProperty");

            migrationBuilder.RenameIndex(
                name: "IX_DescriptionProperties_GreenPointId",
                table: "DescriptionProperty",
                newName: "IX_DescriptionProperty_GreenPointId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GreenPoint",
                table: "GreenPoint",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DescriptionProperty",
                table: "DescriptionProperty",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DescriptionProperty_GreenPoint_GreenPointId",
                table: "DescriptionProperty",
                column: "GreenPointId",
                principalTable: "GreenPoint",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GreenPointUser_GreenPoint_CollaborationsId",
                table: "GreenPointUser",
                column: "CollaborationsId",
                principalTable: "GreenPoint",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
