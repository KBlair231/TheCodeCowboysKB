using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromptQuest.Migrations
{
    /// <inheritdoc />
    public partial class eventNum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventNum",
                table: "GameStates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventNum",
                table: "GameStates");
        }
    }
}
