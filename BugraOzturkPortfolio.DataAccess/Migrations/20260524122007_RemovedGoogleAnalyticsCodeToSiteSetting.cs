using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugraOzturkPortfolio.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemovedGoogleAnalyticsCodeToSiteSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleAnalyticsCode",
                table: "SiteSettings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleAnalyticsCode",
                table: "SiteSettings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
