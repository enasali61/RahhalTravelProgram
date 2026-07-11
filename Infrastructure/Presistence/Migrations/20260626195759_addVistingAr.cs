using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presistence.Migrations
{
    /// <inheritdoc />
    public partial class addVistingAr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VisitingTime",
                table: "Places",
                newName: "VisitingTimeEn");

            migrationBuilder.AddColumn<string>(
                name: "VisitingTimeAr",
                table: "Places",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitingTimeAr",
                table: "Places");

            migrationBuilder.RenameColumn(
                name: "VisitingTimeEn",
                table: "Places",
                newName: "VisitingTime");
        }
    }
}
