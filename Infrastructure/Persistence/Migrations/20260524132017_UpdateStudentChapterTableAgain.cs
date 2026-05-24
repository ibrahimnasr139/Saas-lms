using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentChapterTableAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentChapters_StudentSubjects_SubjectId",
                table: "StudentChapters");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentChapters_AvailableSubjects_SubjectId",
                table: "StudentChapters",
                column: "SubjectId",
                principalTable: "AvailableSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentChapters_AvailableSubjects_SubjectId",
                table: "StudentChapters");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentChapters_StudentSubjects_SubjectId",
                table: "StudentChapters",
                column: "SubjectId",
                principalTable: "StudentSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
