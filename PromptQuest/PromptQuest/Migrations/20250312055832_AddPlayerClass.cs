using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromptQuest.Migrations {
	/// <inheritdoc />
	public partial class AddPlayerClass : Migration {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.AddColumn<string>(
				name: "Class",
				table: "Players",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropColumn(
				name: "Class",
				table: "Players");
		}
	}
}
