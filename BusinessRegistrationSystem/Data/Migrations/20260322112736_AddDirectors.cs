using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessRegistrationSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDirectors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessPeople");

            migrationBuilder.DropColumn(
                name: "Capital",
                table: "BusinessRegistrations");

            migrationBuilder.CreateTable(
                name: "Directors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRegistrationId = table.Column<Guid>(type: "uuid", nullable: false),
                    NIC = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FirstNames = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DivisionalSecretariatDivision = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ResidencePhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MobileNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Occupation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Directors_BusinessRegistrations_BusinessRegistrationId",
                        column: x => x.BusinessRegistrationId,
                        principalTable: "BusinessRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Directors_BusinessRegistrationId",
                table: "Directors",
                column: "BusinessRegistrationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Directors");

            migrationBuilder.AddColumn<decimal>(
                name: "Capital",
                table: "BusinessRegistrations",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BusinessPeople",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRegistrationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DivisionalSecretariatDivision = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstNames = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsDirector = table.Column<bool>(type: "boolean", nullable: false),
                    IsSecretary = table.Column<bool>(type: "boolean", nullable: false),
                    IsShareholder = table.Column<bool>(type: "boolean", nullable: false),
                    MobileNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NIC = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Occupation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ResidencePhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SharesCount = table.Column<int>(type: "integer", nullable: true),
                    Surname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessPeople", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessPeople_BusinessRegistrations_BusinessRegistrationId",
                        column: x => x.BusinessRegistrationId,
                        principalTable: "BusinessRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPeople_BusinessRegistrationId",
                table: "BusinessPeople",
                column: "BusinessRegistrationId");
        }
    }
}
