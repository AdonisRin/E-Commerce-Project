using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftAndAesthetics.Migrations
{
    /// <inheritdoc />
    public partial class Mil4Test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveMembership",
                table: "Members");

            migrationBuilder.AddColumn<DateTime>(
                name: "MembershipExpiryDate",
                table: "Members",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MembershipExpiryDate",
                table: "Members");

            migrationBuilder.AddColumn<string>(
                name: "ActiveMembership",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
