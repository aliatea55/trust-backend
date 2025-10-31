using System.ComponentModel.DataAnnotations.Schema;

namespace TrustInsuranceApi1.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public string NationalId { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? ProfileImageUrl { get; set; }

        public bool IsProfileComplete { get; set; } = false;

        public int GenderId { get; set; }       // المفتاح الأجنبي
        public Gender Gender { get; set; } = null!;  // الكائن المرتبط
   

    }
}
