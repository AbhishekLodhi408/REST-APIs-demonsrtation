using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Application1.Migrations
{
    /// <inheritdoc />
    public partial class Seedingdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "CourseId", "Description", "Duration", "Name", "Price" },
                values: new object[,]
                {
                    { 201, "A well designed react course", "10hrs", "React Course", 0 },
                    { 202, "A well designed nodejs course", "12hrs", "NodeJS Course", 0 },
                    { 203, "A good C# course", "8hrs", "C# Course", 0 },
                    { 204, "A full .Net course", "20hrs", ".Net Course", 0 },
                    { 205, "A well designed angular course", "8hrs", "Angular Course", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 205);
        }
    }
}
