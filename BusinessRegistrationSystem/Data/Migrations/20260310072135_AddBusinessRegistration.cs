using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessRegistrationSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservationName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SinhalaName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TamilName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Abbreviations = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessRegistrations_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRegistrations_OwnerId",
                table: "BusinessRegistrations",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessRegistrations");
        }
    }
}
