namespace Application1.Models.Student
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Standard { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public int ImageId { get; set; }
    }
}
