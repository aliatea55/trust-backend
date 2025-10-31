namespace TrustInsuranceApi1.Dtos
{
    public class RegisterDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string NationalId { get; set; } = string.Empty;
      
        public int GenderId { get; set; }

    }
}
