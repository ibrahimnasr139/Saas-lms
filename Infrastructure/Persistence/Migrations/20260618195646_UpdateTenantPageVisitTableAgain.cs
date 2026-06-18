using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTenantPageVisitTableAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenantPageVisits_TenantId",
                table: "TenantPageVisits");

            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "TenantPageVisits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TenantPageVisits_TenantId_VisitorId_PageUrl",
                table: "TenantPageVisits",
                columns: new[] { "TenantId", "VisitorId", "PageUrl" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenantPageVisits_TenantId_VisitorId_PageUrl",
                table: "TenantPageVisits");

            migrationBuilder.DropColumn(
                name: "Views",
                table: "TenantPageVisits");

            migrationBuilder.CreateIndex(
                name: "IX_TenantPageVisits_TenantId",
                table: "TenantPageVisits",
                column: "TenantId");
        }
    }
}
