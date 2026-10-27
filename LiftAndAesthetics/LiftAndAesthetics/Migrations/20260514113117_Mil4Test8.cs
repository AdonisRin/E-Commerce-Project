using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftAndAesthetics.Migrations
{
    /// <inheritdoc />
    public partial class Mil4Test8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "ChatMessage",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "ChatMessage");
        }
    }
}
