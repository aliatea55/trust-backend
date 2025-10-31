// Models/ProfileUpdateDto.cs
namespace TrustInsuranceApi1.Models
{
    public class ProfileUpdateDto
    {
        public int UserId { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? Image { get; set; }
    }
}
