using Application1.DTOs;
using Application1.Models.Course;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Application1.Repositries
{
    public class CourseRepositry : ICourseRepositry
    {
        private readonly IDbConnection _dbconnection;
        public CourseRepositry(IConfiguration configuration)
        {
            _dbconnection = new SqlConnection(configuration.GetConnectionString("OurConnectionString"));
        }
        public async Task<IEnumerable<Course>> GetAllCourse(bool isUserAdmin)
        {
            var sql = "SELECT * FROM [StudentsDb].[dbo].[Courses]" + (isUserAdmin ? "" : " WHERE IsActive=1");
            var courses = await _dbconnection.QueryAsync<Course>(sql);
            return courses;
        }

        public async Task<Course> GetCourse(int id)
        {
            var sql = "SELECT * FROM [StudentsDb].[dbo].[Courses] WHERE IsActive=1 AND Id=" + id;
            var course = _dbconnection.QueryFirstOrDefault<Course>(sql);
            return course;
        }

        public async Task<int> AddCourse(NewCourseDTO course)
        {
            var sql = @"INSERT INTO [StudentsDb].[dbo].[Courses] ([Name], [Description], [Duration], [Price], [IsActive]) 
                        VALUES (@Name, @Description, @Duration, @Price, @IsActive)";
            var parameters = new
            {
                Name = course.Name,
                Description = course.Description,
                Duration = course.Duration,
                Price = course.Price,
                IsActive = 1
            };
            return await _dbconnection.ExecuteAsync(sql, parameters);
        }

        public async Task<int> UpdateCourse(int id, UpdateCourseDTO course)
        {
            var updates = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            if (course.Name != null)
            {
                updates.Add("Name=@Name");
                parameters.Add("@Name", course.Name);
            }
            if (course.Description != null)
            {
                updates.Add("Description=@Description");
                parameters.Add("@Description", course.Description);
            }
            if (course.Duration != null)
            {
                updates.Add("Duration=@Duration");
                parameters.Add("@Duration", course.Duration);
            }
            if (course.Price != null)
            {
                updates.Add("Price=@Price");
                parameters.Add("@Price", course.Price);
            }
            if (course.IsActive != null)
            {
                updates.Add("IsActive=@IsActive");
                parameters.Add("@IsActive", course.IsActive == true ? '1' : '0');
            }

            if (!updates.Any())
            {
                throw new Exception("No valid fields provided to update.");
            }

            var sql = $"UPDATE [StudentsDb].[dbo].[Courses] SET {string.Join(", ", updates)} WHERE Id = @Id";
            return await _dbconnection.ExecuteAsync(sql, parameters);
        }

        public async Task<int> EnrollCourse(CourseEnrollDTO details)
        {
            var sql = @"INSERT INTO [StudentsDb].[dbo].[Enrollments] ([Student_Id]
                      ,[Course_Id] ,[IsActive]) Values(" + details.Student_Id + ", "
                      + details.Course_Id + ", " + (details.Is_Active != null ? (details.Is_Active == true ? 1 : 0) : 1) + ")";
            return await _dbconnection.ExecuteAsync(sql);
        }

        public async Task<IEnumerable<Course>> GetRegisteredCourse(int id)
        {
            var sql = @"SELECT * FROM [StudentsDb].[dbo].[Courses] 
                        WHERE Id IN (SELECT Course_Id FROM  [StudentsDb].[dbo].[Enrollments] 
                        WHERE Student_Id=@Id)";
            var parameters = new
            {
                Id = id,
            };
            var result = await _dbconnection.QueryAsync<Course>(sql, parameters);
            return result;
        }
    }
}
