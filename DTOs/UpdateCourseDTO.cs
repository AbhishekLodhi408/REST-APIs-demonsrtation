namespace Application1.DTOs
{
    public class UpdateCourseDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Duration { get; set; }
        public int? Price { get; set; }
        public bool? IsActive { get; set; }
    }
}
