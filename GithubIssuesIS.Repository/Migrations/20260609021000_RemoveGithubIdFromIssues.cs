using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GithubIssuesIS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGithubIdFromIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Issues_GithubId",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "GithubId",
                table: "Issues");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "GithubId",
                table: "Issues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Issues_GithubId",
                table: "Issues",
                column: "GithubId");
        }
    }
}
