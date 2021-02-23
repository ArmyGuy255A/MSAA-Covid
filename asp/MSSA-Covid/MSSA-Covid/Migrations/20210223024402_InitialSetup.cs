using Microsoft.EntityFrameworkCore.Migrations;

namespace MSSA_Covid.Migrations
{
    public partial class InitialSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KioskConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KioskConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    County = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageBlobUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PageStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hits = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KioskFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlobUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KioskConfigurationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KioskFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KioskFiles_KioskConfigurations_KioskConfigurationId",
                        column: x => x.KioskConfigurationId,
                        principalTable: "KioskConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "KioskConfigurations",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { -1, "Kiosk Description", "Default" });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "City", "County", "ImageBlobUrl", "ImageName", "State", "Url" },
                values: new object[,]
                {
                    { -10, "Pensacola", "Escambia", null, null, "Florida", null },
                    { -1, "Fort Walton Beach", "Okaloosa", null, null, "Florida", null },
                    { -2, "Crestview", "Okaloosa", null, null, "Florida", null },
                    { -4, "Niceville", "Okaloosa", null, null, "Florida", null },
                    { -5, "Destin", "Okaloosa", null, null, "Florida", null },
                    { -6, "Hephzibah", "Richmond", null, null, "Georgia", null },
                    { -7, "Augusta", "Richmond", null, null, "Georgia", null },
                    { -8, "Fort Gordon", "Richmond", null, null, "Georgia", null },
                    { -9, "Blythe", "Richmond", null, null, "Georgia", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_KioskFiles_KioskConfigurationId",
                table: "KioskFiles",
                column: "KioskConfigurationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KioskFiles");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "PageStatistics");

            migrationBuilder.DropTable(
                name: "KioskConfigurations");
        }
    }
}
