using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugraOzturkPortfolio.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_About_SocialMedia_Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                table: "Abouts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                table: "Abouts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedinUrl",
                table: "Abouts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwitterUrl",
                table: "Abouts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                table: "Abouts");

            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                table: "Abouts");

            migrationBuilder.DropColumn(
                name: "LinkedinUrl",
                table: "Abouts");

            migrationBuilder.DropColumn(
                name: "TwitterUrl",
                table: "Abouts");
        }
    }
}
