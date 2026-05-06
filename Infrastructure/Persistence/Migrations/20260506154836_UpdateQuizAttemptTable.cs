using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuizAttemptTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuizAttempts_StudentId",
                table: "QuizAttempts");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_StudentId_ModuleItemId",
                table: "QuizAttempts",
                columns: new[] { "StudentId", "ModuleItemId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuizAttempts_StudentId_ModuleItemId",
                table: "QuizAttempts");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_StudentId",
                table: "QuizAttempts",
                column: "StudentId",
                unique: true);
        }
    }
}
