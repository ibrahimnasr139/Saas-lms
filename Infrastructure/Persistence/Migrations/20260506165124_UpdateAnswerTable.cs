using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnswerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_QuizAttempts_QuizAttemptId",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_QuizAttemptId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "QuizAttemptId",
                table: "Answers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuizAttemptId",
                table: "Answers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuizAttemptId",
                table: "Answers",
                column: "QuizAttemptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_QuizAttempts_QuizAttemptId",
                table: "Answers",
                column: "QuizAttemptId",
                principalTable: "QuizAttempts",
                principalColumn: "Id");
        }
    }
}
