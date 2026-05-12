namespace QuickBite.Auth.Application.DTOs
{
    public class UpdateProfileRequestDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
    }
}
