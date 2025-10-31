namespace TrustInsuranceApi1.Models
{
    public class Gender
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<User> Users { get; set; } = new(); // ✅ تهيئة لتفادي التحذير
    }
}
