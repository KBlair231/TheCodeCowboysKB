using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromptQuest.Migrations
{
    /// <inheritdoc />
    public partial class AddedFloorToGameState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Floor",
                table: "GameStates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Floor",
                table: "GameStates");
        }
    }
}
