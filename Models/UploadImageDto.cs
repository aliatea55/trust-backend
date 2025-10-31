namespace TrustInsuranceApi1.Dtos
{
    public class UploadImageDto
    {
        public int UserId { get; set; }
        public IFormFile? Image { get; set; }
    }
}
