using Application1.DTOs;
using Application1.Models.Student;

namespace Application1.Repositries
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllStudents(bool isAdmin);
        Task<Student> GetStudent(int id);
        Task<int> Register(StudentDTO student);
        Task<int> UpdateStudent(int id, StudentUpdateDTO student);
        Task<int> DeleteStudent(int id);
        Task<int> SaveImage(Image image);

        Task<Image> GetImage(int id);
    }
}
