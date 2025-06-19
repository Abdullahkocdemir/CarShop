using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Mig_add_WhyUse_add_deneme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WhyUseReason_WhyUses_WhyUseId",
                table: "WhyUseReason");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WhyUseReason",
                table: "WhyUseReason");

            migrationBuilder.RenameTable(
                name: "WhyUseReason",
                newName: "WhyUseReasons");

            migrationBuilder.RenameIndex(
                name: "IX_WhyUseReason_WhyUseId",
                table: "WhyUseReasons",
                newName: "IX_WhyUseReasons_WhyUseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WhyUseReasons",
                table: "WhyUseReasons",
                column: "WhyUseReasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_WhyUseReasons_WhyUses_WhyUseId",
                table: "WhyUseReasons",
                column: "WhyUseId",
                principalTable: "WhyUses",
                principalColumn: "WhyUseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WhyUseReasons_WhyUses_WhyUseId",
                table: "WhyUseReasons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WhyUseReasons",
                table: "WhyUseReasons");

            migrationBuilder.RenameTable(
                name: "WhyUseReasons",
                newName: "WhyUseReason");

            migrationBuilder.RenameIndex(
                name: "IX_WhyUseReasons_WhyUseId",
                table: "WhyUseReason",
                newName: "IX_WhyUseReason_WhyUseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WhyUseReason",
                table: "WhyUseReason",
                column: "WhyUseReasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_WhyUseReason_WhyUses_WhyUseId",
                table: "WhyUseReason",
                column: "WhyUseId",
                principalTable: "WhyUses",
                principalColumn: "WhyUseId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
