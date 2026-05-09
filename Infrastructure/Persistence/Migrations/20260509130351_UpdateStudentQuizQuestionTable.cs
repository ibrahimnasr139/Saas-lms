using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentQuizQuestionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Option",
                table: "StudentQuizQuestions");

            migrationBuilder.AddColumn<string>(
                name: "Options",
                table: "StudentQuizQuestions",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Options",
                table: "StudentQuizQuestions");

            migrationBuilder.AddColumn<int>(
                name: "Option",
                table: "StudentQuizQuestions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
