namespace TrustInsuranceApi1.Models
{
    public class GenderDto
    {
        public int Value { get; set; } // ✅ عدل من string إلى int
        public string Label { get; set; } = string.Empty;
    }
}
