using Application1.Models.Course;
using Application1.Models.Student;
using Microsoft.EntityFrameworkCore;
namespace Application1.Data
{
    public class Student_Course_DBContext : DbContext
    {
        public Student_Course_DBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Data to seed
            var courses = new List<Course>()
            {
              new Course()
              {
                  Id = 201,
                  Name = "React Course",
                  Description = "A well designed react course",
                  Duration = "10hrs"
              },
              new Course()
              {
                  Id = 202,
                  Name = "NodeJS Course",
                  Description = "A well designed nodejs course",
                  Duration = "12hrs"
              },
              new Course()
              {
                  Id = 203,
                  Name = "C# Course",
                  Description = "A good C# course",
                  Duration = "8hrs"
              },
              new Course()
              {
                  Id = 204,
                  Name = ".Net Course",
                  Description = "A full .Net course",
                  Duration = "20hrs"
              },
              new Course()
              {
                  Id = 205,
                  Name = "Angular Course",
                  Description = "A well designed angular course",
                  Duration = "8hrs"
              }
            };

            //Seed the data
            modelBuilder.Entity<Course>().HasData(courses);
        }
    }
}
