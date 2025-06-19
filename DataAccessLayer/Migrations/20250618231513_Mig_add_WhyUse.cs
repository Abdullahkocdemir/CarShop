using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Mig_add_WhyUse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SmallTitle",
                table: "WhyUses",
                newName: "VideoUrl");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "WhyUses",
                newName: "VideoImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "MainDescription",
                table: "WhyUses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainTitle",
                table: "WhyUses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "WhyUseReason",
                columns: table => new
                {
                    WhyUseReasonId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReasonText = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IconCssClass = table.Column<string>(type: "text", nullable: false),
                    WhyUseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhyUseReason", x => x.WhyUseReasonId);
                    table.ForeignKey(
                        name: "FK_WhyUseReason_WhyUses_WhyUseId",
                        column: x => x.WhyUseId,
                        principalTable: "WhyUses",
                        principalColumn: "WhyUseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WhyUseReason_WhyUseId",
                table: "WhyUseReason",
                column: "WhyUseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhyUseReason");

            migrationBuilder.DropColumn(
                name: "MainDescription",
                table: "WhyUses");

            migrationBuilder.DropColumn(
                name: "MainTitle",
                table: "WhyUses");

            migrationBuilder.RenameColumn(
                name: "VideoUrl",
                table: "WhyUses",
                newName: "SmallTitle");

            migrationBuilder.RenameColumn(
                name: "VideoImageUrl",
                table: "WhyUses",
                newName: "Description");
        }
    }
}
