using Application1.DTOs;
using Application1.Models.Course;
using Microsoft.AspNetCore.Mvc;

namespace Application1.Repositries
{
    public interface ICourseRepositry
    {
        Task<IEnumerable<Course>> GetAllCourse(bool isUserAdmin);
        Task<Course> GetCourse(int id);
        Task<int> AddCourse(NewCourseDTO course);
        Task<int> UpdateCourse(int id, UpdateCourseDTO course);
        Task<int> EnrollCourse(CourseEnrollDTO details);
        Task<IEnumerable<Course>> GetRegisteredCourse(int id);
    }
}
