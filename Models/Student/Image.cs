namespace Application1.Models.Student
{
    public class Image
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] ImageData { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }

    }
}
