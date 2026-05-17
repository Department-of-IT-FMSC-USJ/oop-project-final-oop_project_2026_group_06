using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessRegistrationSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "BusinessRegistrations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyEmail",
                table: "BusinessRegistrations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyPhoneNumber",
                table: "BusinessRegistrations",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "BusinessRegistrations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DivisionalSecretariatDivision",
                table: "BusinessRegistrations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GNDivision",
                table: "BusinessRegistrations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Objectives",
                table: "BusinessRegistrations",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "BusinessRegistrations",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "BusinessRegistrations");

            migrationBuilder.DropColumn(
                name: "CompanyEmail",
                table: "BusinessRegistrations");

            migrationBuilder.DropColumn(
                name: "CompanyPhoneNumber",
                table: "BusinessRegistrations");

            migrationBuilder.DropColumn(
                name: "District",
                table: "BusinessRegistrations");

            migrationBuilder.DropColumn(
                name: "DivisionalSecretariatDivision",
                table: "BusinessRegistrations");

            migrationBuilder.DropColumn(
                name: "GNDivision",
                table: "BusinessRegistrations");

            migrationBuilder.DropColumn(
                name: "Objectives",
                table: "BusinessRegistrations");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "BusinessRegistrations");
        }
    }
}
