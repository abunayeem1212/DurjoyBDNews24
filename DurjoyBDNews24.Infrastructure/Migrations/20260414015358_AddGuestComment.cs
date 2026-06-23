using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DurjoyBDNews24.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuestComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuestEmail",
                schema: "dbo",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuestName",
                schema: "dbo",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuestEmail",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "GuestName",
                schema: "dbo",
                table: "Comments");
        }
    }
}
