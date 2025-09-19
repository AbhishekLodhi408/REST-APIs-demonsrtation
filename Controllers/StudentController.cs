using Application1.Data;
using Application1.DTOs;
using Application1.Models.Course;
using Application1.Models.Student;
using Application1.Repositries;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Application1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly Student_Course_DBContext dbContext;
        DataContextDappper dataContextDapper;
        private readonly IStudentRepository _studentRepository;
        public StudentController(Student_Course_DBContext student_Course_DBContext, IConfiguration config, IStudentRepository studentRepository)
        {
            this.dbContext = student_Course_DBContext;
            dataContextDapper = new DataContextDappper(config);
            _studentRepository = studentRepository; 
        }

        //[HttpGet]
        //[Route("/testDb")]
        //public DateTime TestDb()
        //{
        //    return dataContextDapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
        //}

        //Get all students
        //Route : student/getall
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll(bool? isAdmin)
        {
            //var students = dbContext.Students.ToList();
            var isUserAdmin = isAdmin ?? false;
            var students = await _studentRepository.GetAllStudents(isUserAdmin);
            return Ok(students);
            
        }

        //Get student details : get current user details 
        //Route : student/getuser
        [HttpGet("getstudent")]
        public async Task<IActionResult> GetStudent()
        {
            //dbContext.Students.Add(student);
            //dbContext.SaveChanges();
            var userid = Convert.ToInt32(User.FindFirst("userId")?.Value);
            var student = await _studentRepository.GetStudent(userid);
            return Ok(student);
        }

        //Post : Upadte the record of student
        //Route : student/update/{id}
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StudentUpdateDTO student) {

            var result = await _studentRepository.UpdateStudent(id, student);
            if (result>0) 
            {
                return Ok("Data updated successfully");
            }
            else
            {
                return NotFound("User Not Found");
            }
        }

        //Post : Delete student
        //Route : student/delete/{id}
        [HttpDelete("delete/{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Delete(int id) {
            var result = await _studentRepository.DeleteStudent(id);
            if (result > 0)
            {
                return Ok("User deleted successfully");
            }
            else
            {
                return NotFound("User Not Found");
            }    
        }

        [HttpPost("upload/image")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            // kal likhte hai...🙂
            if (image == null || image.Length == 0)
                return BadRequest("File not selected.");

            //if(image.ContentType !=='')

            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);

            var img = new Image
            {
                FileName = image.FileName,
                ContentType = image.ContentType,
                CreatedDate = DateTime.Now,
                ImageData = ms.ToArray(),
                CreatedBy = Convert.ToInt32(User.FindFirst("userId")?.Value)
            };

            var id = await _studentRepository.SaveImage(img);

            return Ok(new { Id = id, Message = "Image uploaded successfully." });
        }

        [HttpGet]
        [Route("get/image/{imageId}")]
        public async Task<IActionResult> GetImage(int imageId)
        {
            var id = Convert.ToInt32(User.FindFirst("userId")?.Value);
            var result =await _studentRepository.GetImage(imageId);
            return Ok(result);
        }

    }
}
