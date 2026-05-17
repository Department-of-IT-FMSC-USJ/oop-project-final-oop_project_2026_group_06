using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessRegistrationSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddShareholdersAndCapital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsShareholder",
                table: "Directors",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfShares",
                table: "Directors",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCapital",
                table: "BusinessRegistrations",
                type: "numeric(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShareholder",
                table: "Directors");

            migrationBuilder.DropColumn(
                name: "NumberOfShares",
                table: "Directors");

            migrationBuilder.DropColumn(
                name: "TotalCapital",
                table: "BusinessRegistrations");
        }
    }
}
