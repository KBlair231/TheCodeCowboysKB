using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromptQuest.Migrations
{
    /// <inheritdoc />
    public partial class f107passives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Passive",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Passive",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Passive",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Passive",
                table: "Items");
        }
    }
}
