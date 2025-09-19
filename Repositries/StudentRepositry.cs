using Application1.DTOs;
using Application1.Models.Student;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Application1.Repositries
{
    public class StudentRepositry : IStudentRepository
    {
        private readonly IDbConnection _dbConnection;
        public StudentRepositry(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("OurConnectionString"));
        }

        public async Task<IEnumerable<Student>>GetAllStudents(bool isAdmin){
            var sql = "SELECT [Id], [Name], [Email], [Standard], [IsActive], [IsAdmin], [ImageId] FROM [StudentsDb].[dbo].[Students]" + (isAdmin ? "" : " WHERE IsActive=1");
            var students = await _dbConnection.QueryAsync<Student>(sql);
            return students;
        }

        public async Task<Student>GetStudent(int id)
        {
            var sql = "SELECT * FROM [StudentsDb].[dbo].[Students] WHERE Id=" + id;
            var student = await _dbConnection.QueryFirstOrDefaultAsync<Student>(sql);
            return student;
        }

        public async Task<int> Register(StudentDTO student)
        {
            var sql = @"INSERT INTO [StudentsDb].[dbo].[Students] ([Name], [Email], [Standard], [IsActive], [PasswordSalt], [PasswordHash])                
                        VALUES (@Name, @Email, @Standard, @IsActive)";

            var parameters = new
            {
                Name= student.Name,
                Email= student.Email,
                Standard = student.Standard,
                IsActive=1
            };

            return await _dbConnection.ExecuteAsync(sql, parameters);
        }

        public async Task<int> UpdateStudent(int id, StudentUpdateDTO student)
        {
            var updates = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            if (student.Name != null)
            {
                updates.Add("name = @Name");
                parameters.Add("@Name", student.Name);
            }

            if (student.Email != null)
            {
                updates.Add("email = @Email");
                parameters.Add("@Email", student.Email);
            }

            if (student.Standard != null)
            {
                updates.Add("standard = @Standard");
                parameters.Add("@Standard", student.Standard);
            }

            if (student.IsActive != null)
            {
                updates.Add("isActive = @IsActive");
                parameters.Add("@IsActive", student.IsActive == true ? 1 : 0);
            }

            if (!updates.Any())
            {
                throw new Exception("No valid fields provided to update.");
            }

            var sql = $"UPDATE Students SET {string.Join(", ", updates)} WHERE Id = @Id";
            return await _dbConnection.ExecuteAsync(sql,parameters);
        }

        public async Task<int> DeleteStudent(int id)
        {
            var sql = "UPDATE [StudentsDb].[dbo].[Students] SET IsActive = 0 WHERE Id=" + id;
            return await _dbConnection.ExecuteAsync(sql);
        }

        public async Task<int> SaveImage(Image image)
        {
            var sql = @"INSERT INTO [StudentsDb].[dbo].[Images] (FileName, ContentType, ImageData, CreatedDate, CreatedBy)
                        VALUES(@Name, @ContentType, @ImageData, GETDATE(), @CreatedBy)
                        SELECT CAST(SCOPE_IDENTITY() as int)";

            var parameters = new
            {
                Name = image.FileName,
                ContentType = image.ContentType,
                ImageData = image.ImageData,
                CreatedBy = image.CreatedBy
            };

            var id = await _dbConnection.ExecuteAsync(sql, parameters);
            return id;
        }

        public async Task<Image> GetImage(int id)
        {
            var sql = @"SELECT Id, FileName, ContentType, ImageData, CreatedDate, CreatedBy
                    FROM [StudentsDb].[dbo].[Images]
                    WHERE Id = " + id;

            return await _dbConnection.QuerySingleOrDefaultAsync<Image>(sql);

        }

    }
}
