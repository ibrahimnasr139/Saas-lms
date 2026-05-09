using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentQuizAndStudentQuizAttemptTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentQuizzes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TimeLimit = table.Column<int>(type: "integer", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    ChapterId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentQuizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentQuizzes_StudentChapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "StudentChapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentQuizzes_StudentSubjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "StudentSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentQuizzes_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentQuizAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Score = table.Column<byte>(type: "smallint", nullable: false),
                    TimeSpent = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    StudentQuizId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentQuizAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentQuizAttempts_StudentQuizzes_StudentQuizId",
                        column: x => x.StudentQuizId,
                        principalTable: "StudentQuizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentQuizAttempts_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentQuizQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Question = table.Column<string>(type: "text", nullable: false),
                    Explanation = table.Column<string>(type: "text", nullable: false),
                    Option = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    StudentQuizId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentQuizQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentQuizQuestions_StudentQuizzes_StudentQuizId",
                        column: x => x.StudentQuizId,
                        principalTable: "StudentQuizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentAnswerText = table.Column<string>(type: "text", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    StudentQuizQuestionId = table.Column<int>(type: "integer", nullable: false),
                    StudentQuizAttemptId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_StudentQuizAttempts_StudentQuizAttemptId",
                        column: x => x.StudentQuizAttemptId,
                        principalTable: "StudentQuizAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_StudentQuizQuestions_StudentQuizQuestionId",
                        column: x => x.StudentQuizQuestionId,
                        principalTable: "StudentQuizQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_StudentQuizAttemptId",
                table: "StudentAnswers",
                column: "StudentQuizAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_StudentQuizQuestionId",
                table: "StudentAnswers",
                column: "StudentQuizQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentQuizAttempts_StudentId",
                table: "StudentQuizAttempts",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentQuizAttempts_StudentQuizId",
                table: "StudentQuizAttempts",
                column: "StudentQuizId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentQuizQuestions_StudentQuizId",
                table: "StudentQuizQuestions",
                column: "StudentQuizId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentQuizzes_ChapterId",
                table: "StudentQuizzes",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentQuizzes_StudentId",
                table: "StudentQuizzes",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentQuizzes_SubjectId",
                table: "StudentQuizzes",
                column: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAnswers");

            migrationBuilder.DropTable(
                name: "StudentQuizAttempts");

            migrationBuilder.DropTable(
                name: "StudentQuizQuestions");

            migrationBuilder.DropTable(
                name: "StudentQuizzes");
        }
    }
}
