namespace Application1.DTOs
{
    public class LoginDetailsDTO
    {
        public string Email { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
    }
}
