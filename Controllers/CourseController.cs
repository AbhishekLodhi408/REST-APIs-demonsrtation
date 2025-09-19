using Application1.Data;
using Application1.DTOs;
using Application1.Models.Course;
using Application1.Models.Student;
using Application1.Repositries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Application1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly Student_Course_DBContext dbContext;
        private readonly DataContextDappper _dapper;
        private readonly ICourseRepositry _courseRepositry;
        public CourseController(Student_Course_DBContext student_Course_DBContext, IConfiguration config, ICourseRepositry courseRepositry)
        {
            this.dbContext = student_Course_DBContext;
            _dapper = new DataContextDappper(config);
            _courseRepositry = courseRepositry;
        }

        public Student_Course_DBContext Student_Course_DBContext { get; }

        //Get all courses 
        //Route : /course/getall
        [AllowAnonymous]
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllCourse(bool? isAdmin)
        {
            //var courses = dbContext.Courses.ToList();
            var isUserAdmin = isAdmin ?? false;
            var courses = await _courseRepositry.GetAllCourse(isUserAdmin);
            return Ok(courses);
        }

        //Get by id
        //Route : /course/id
        [AllowAnonymous]
        [HttpGet("course/{id}")]
        public async Task<IActionResult> GetCourse(int id) {
            //var course = dbContext.Courses.SingleOrDefault(c => c.Id == id);

            var course = await _courseRepositry.GetCourse(id);
            return Ok(course);
        }

        //Post : Add new course
        //Route : course/addCourse
        [HttpPost("addCourse")]
        public async Task<IActionResult> AddCourse([FromBody] NewCourseDTO course) 
        {

            if (course == null || course.Name == null || course.Price == null)
            {
                return BadRequest("Please send required details");
            }
            var result = await _courseRepositry.AddCourse(course);
            if (result > 0)
            {
                return Ok("Course added successfully");
            }
            else
            {
                return BadRequest("Unable to add course or something went wrong");
            }

        }

        //Patch : Update details of course
        //Route : /course/updateCourse/id
        [HttpPatch("updateCourse/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseDTO course)
        {
            var result = await _courseRepositry.UpdateCourse(id, course);
            if (result > 0)
            {
                return Ok("Course updated successfully");
            }
            else
            {
                return BadRequest("Unable to update course.");
            }

        }

        //Post : Enroll new course
        //Route : course/enrollCourse
        [HttpPost("enrollCourse")]
            public async Task<IActionResult> EnrollCourse([FromBody] CourseEnrollDTO details)
            {
                if (details == null || details.Student_Id == null || details.Course_Id == null)
                {
                    return BadRequest("Please provide required details");
                }
                var result = await _courseRepositry.EnrollCourse(details);
                if (result > 0)
                {
                    return Ok("Enrolled successfully");
                }
                else
                {
                    throw new Exception("Unable to enroll user");
                }
            }

        //Get : get course by id
        //Route : course/getregisteredcourse/{id}
        [HttpGet("getregisteredcourse/{id}")]
            public async Task<IActionResult> GetRegisteredCourses(int id)
            {
                var result = await _courseRepositry.GetRegisteredCourse(id);
                return Ok(result);
            }
        
    }
}
