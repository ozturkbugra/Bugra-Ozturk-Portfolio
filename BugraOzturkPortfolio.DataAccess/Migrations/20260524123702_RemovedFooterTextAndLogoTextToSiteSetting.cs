using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugraOzturkPortfolio.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemovedFooterTextAndLogoTextToSiteSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FooterText",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "LogoText",
                table: "SiteSettings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FooterText",
                table: "SiteSettings",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LogoText",
                table: "SiteSettings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
