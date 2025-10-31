namespace TrustInsuranceApi1.Dtos
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public int GenderId { get; set; }

        public DateTime? BirthDate { get; set; }
        public string? NationalId { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
