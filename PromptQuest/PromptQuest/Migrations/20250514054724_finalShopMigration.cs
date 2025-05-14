using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromptQuest.Migrations
{
    /// <inheritdoc />
    public partial class finalShopMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Defense",
                table: "Players",
                newName: "BaseDefense");

            migrationBuilder.RenameColumn(
                name: "Attack",
                table: "Players",
                newName: "BaseAttack");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BaseDefense",
                table: "Players",
                newName: "Defense");

            migrationBuilder.RenameColumn(
                name: "BaseAttack",
                table: "Players",
                newName: "Attack");
        }
    }
}
