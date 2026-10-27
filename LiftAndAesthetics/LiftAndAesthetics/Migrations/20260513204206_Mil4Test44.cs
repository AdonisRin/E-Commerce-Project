using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftAndAesthetics.Migrations
{
    /// <inheritdoc />
    public partial class Mil4Test44 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "Members");
        }
    }
}
