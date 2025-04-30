using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromptQuest.Migrations
{
    /// <inheritdoc />
    public partial class Sprint5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AbilityCooldown",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefenseBuff",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StatusEffects",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusEffects",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "itemType",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "InTreasure",
                table: "GameStates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StoredMapNodeIdsVisited",
                table: "GameStates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StatusEffects",
                table: "Enemies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbilityCooldown",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "DefenseBuff",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "StatusEffects",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "StatusEffects",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "itemType",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "InTreasure",
                table: "GameStates");

            migrationBuilder.DropColumn(
                name: "StoredMapNodeIdsVisited",
                table: "GameStates");

            migrationBuilder.DropColumn(
                name: "StatusEffects",
                table: "Enemies");
        }
    }
}
