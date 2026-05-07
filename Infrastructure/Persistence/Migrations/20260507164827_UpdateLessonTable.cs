using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLessonTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Files_FileId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_FileId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Lessons");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_VideoId",
                table: "Lessons",
                column: "VideoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Files_VideoId",
                table: "Lessons",
                column: "VideoId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Files_VideoId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_VideoId",
                table: "Lessons");

            migrationBuilder.AddColumn<string>(
                name: "FileId",
                table: "Lessons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_FileId",
                table: "Lessons",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Files_FileId",
                table: "Lessons",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
