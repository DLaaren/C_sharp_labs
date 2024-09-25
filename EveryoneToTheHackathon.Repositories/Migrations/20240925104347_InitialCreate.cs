using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EveryoneToTheHackathon.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => new { x.Id, x.Title });
                });

            migrationBuilder.CreateTable(
                name: "Hackathons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MeanSatisfactionIndex = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hackathons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeHackathon",
                columns: table => new
                {
                    HackathonsId = table.Column<int>(type: "integer", nullable: false),
                    EmployeesId = table.Column<int>(type: "integer", nullable: false),
                    EmployeesTitle = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeHackathon", x => new { x.HackathonsId, x.EmployeesId, x.EmployeesTitle });
                    table.ForeignKey(
                        name: "FK_EmployeeHackathon_Employees_EmployeesId_EmployeesTitle",
                        columns: x => new { x.EmployeesId, x.EmployeesTitle },
                        principalTable: "Employees",
                        principalColumns: new[] { "Id", "Title" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeHackathon_Hackathons_HackathonsId",
                        column: x => x.HackathonsId,
                        principalTable: "Hackathons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HackathonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Hackathons_HackathonId",
                        column: x => x.HackathonId,
                        principalTable: "Hackathons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    EmployeeTitle = table.Column<string>(type: "text", nullable: false),
                    DesiredEmployees = table.Column<int[]>(type: "integer[]", nullable: false),
                    HackathonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wishlists_Employees_EmployeeId_EmployeeTitle",
                        columns: x => new { x.EmployeeId, x.EmployeeTitle },
                        principalTable: "Employees",
                        principalColumns: new[] { "Id", "Title" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wishlists_Hackathons_HackathonId",
                        column: x => x.HackathonId,
                        principalTable: "Hackathons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTeam",
                columns: table => new
                {
                    TeamsId = table.Column<int>(type: "integer", nullable: false),
                    EmployeesId = table.Column<int>(type: "integer", nullable: false),
                    EmployeesTitle = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTeam", x => new { x.TeamsId, x.EmployeesId, x.EmployeesTitle });
                    table.ForeignKey(
                        name: "FK_EmployeeTeam_Employees_EmployeesId_EmployeesTitle",
                        columns: x => new { x.EmployeesId, x.EmployeesTitle },
                        principalTable: "Employees",
                        principalColumns: new[] { "Id", "Title" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeTeam_Teams_TeamsId",
                        column: x => x.TeamsId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHackathon_EmployeesId_EmployeesTitle",
                table: "EmployeeHackathon",
                columns: new[] { "EmployeesId", "EmployeesTitle" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTeam_EmployeesId_EmployeesTitle",
                table: "EmployeeTeam",
                columns: new[] { "EmployeesId", "EmployeesTitle" });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_HackathonId",
                table: "Teams",
                column: "HackathonId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_EmployeeId_EmployeeTitle",
                table: "Wishlists",
                columns: new[] { "EmployeeId", "EmployeeTitle" });

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_HackathonId",
                table: "Wishlists",
                column: "HackathonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeHackathon");

            migrationBuilder.DropTable(
                name: "EmployeeTeam");

            migrationBuilder.DropTable(
                name: "Wishlists");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Hackathons");
        }
    }
}
