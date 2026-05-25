using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugraOzturkPortfolio.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAboutStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTitle",
                table: "Abouts");

            migrationBuilder.AlterColumn<string>(
                name: "CvUrl",
                table: "Abouts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "GithubUrl",
                table: "Abouts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GithubUrl",
                table: "Abouts");

            migrationBuilder.AlterColumn<string>(
                name: "CvUrl",
                table: "Abouts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubTitle",
                table: "Abouts",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }
    }
}
