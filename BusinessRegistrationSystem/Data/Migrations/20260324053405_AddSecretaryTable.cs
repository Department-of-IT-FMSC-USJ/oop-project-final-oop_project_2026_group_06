using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessRegistrationSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSecretaryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Secretaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRegistrationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SecretaryType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NIC = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Title = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    FirstNames = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Surname = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secretaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Secretaries_BusinessRegistrations_BusinessRegistrationId",
                        column: x => x.BusinessRegistrationId,
                        principalTable: "BusinessRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Secretaries_BusinessRegistrationId",
                table: "Secretaries",
                column: "BusinessRegistrationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Secretaries");
        }
    }
}
