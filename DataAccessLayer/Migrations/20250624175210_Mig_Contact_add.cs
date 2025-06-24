using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Mig_Contact_add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Contacts");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Contacts",
                newName: "NameSurName");

            migrationBuilder.RenameColumn(
                name: "SurName",
                table: "Contacts",
                newName: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NameSurName",
                table: "Contacts",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Contacts",
                newName: "SurName");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Contacts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
