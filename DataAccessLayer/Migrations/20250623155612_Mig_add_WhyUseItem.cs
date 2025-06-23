using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Mig_add_WhyUseItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhyUseReasons");

            migrationBuilder.CreateTable(
                name: "WhyUseItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WhyUseId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhyUseItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WhyUseItems_WhyUses_WhyUseId",
                        column: x => x.WhyUseId,
                        principalTable: "WhyUses",
                        principalColumn: "WhyUseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WhyUseItems_WhyUseId",
                table: "WhyUseItems",
                column: "WhyUseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhyUseItems");

            migrationBuilder.CreateTable(
                name: "WhyUseReasons",
                columns: table => new
                {
                    WhyUseReasonId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WhyUseId = table.Column<int>(type: "integer", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IconCssClass = table.Column<string>(type: "text", nullable: false),
                    ReasonText = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhyUseReasons", x => x.WhyUseReasonId);
                    table.ForeignKey(
                        name: "FK_WhyUseReasons_WhyUses_WhyUseId",
                        column: x => x.WhyUseId,
                        principalTable: "WhyUses",
                        principalColumn: "WhyUseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WhyUseReasons_WhyUseId",
                table: "WhyUseReasons",
                column: "WhyUseId");
        }
    }
}
