using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftAndAesthetics.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FitnessClasses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraPhoto1Url",
                table: "FitnessClasses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraPhoto2Url",
                table: "FitnessClasses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainPhotoUrl",
                table: "FitnessClasses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "FitnessClasses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "FitnessClasses");

            migrationBuilder.DropColumn(
                name: "ExtraPhoto1Url",
                table: "FitnessClasses");

            migrationBuilder.DropColumn(
                name: "ExtraPhoto2Url",
                table: "FitnessClasses");

            migrationBuilder.DropColumn(
                name: "MainPhotoUrl",
                table: "FitnessClasses");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "FitnessClasses");
        }
    }
}
