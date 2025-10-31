using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrustInsuranceApi1.Data;
using TrustInsuranceApi1.Models;
using TrustInsuranceApi1.Dtos;

namespace TrustInsuranceApi1.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest(new { message = "البريد الإلكتروني مستخدم بالفعل" });

            var newUser = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Password = dto.Password,
                BirthDate = dto.BirthDate ?? DateTime.Now,
                NationalId = dto.NationalId,
                GenderId = dto.GenderId, // حفظ فقط رقم الجنس
                IsProfileComplete = true // ✅ لو المستخدم دخل كل البيانات من البداية

            };



            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(newUser);
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email && u.Password == dto.Password);
            if (user == null)
                return Unauthorized(new { message = "البريد الإلكتروني أو كلمة المرور غير صحيحة" });

            return Ok(new
            {
                user = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.Gender,
                    user.BirthDate,
                    user.NationalId,
                    user.Address,
                    ProfileImageUrl = user.ProfileImageUrl, // ✅ تم الإضافة
                    isProfileComplete = user.Email == "ali.admin@trust.com" ? true : user.IsProfileComplete
                }
            });
        }


        [HttpPost("complete-profile")]
        public async Task<IActionResult> CompleteProfile([FromForm] ProfileUpdateDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);
            if (user == null)
                return NotFound();

            if (dto.Image != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                user.ProfileImageUrl = "/uploads/" + fileName;
            }

            user.Address = dto.Address;
            user.PhoneNumber = dto.PhoneNumber;
            user.IsProfileComplete = true;

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users
                .Include(u => u.Gender) // ✅ تحميل معلومات الجنس
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound(new { message = "المستخدم غير موجود" });

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.NationalId,
                user.BirthDate,
                GenderId = user.GenderId,
                Gender = user.Gender?.Name, // ✅ عرض اسم الجنس بدلًا من رقمه
                user.ProfileImageUrl
            });
        }


        [HttpGet("all")]
        public IActionResult GetAllUsers([FromHeader(Name = "IsAdminEmail")] string email)
        {
            if (email != "ali.admin@trust.com")
                return Unauthorized(new { message = "أنت غير مصرح لك بعرض المستخدمين" });

            var users = _context.Users
                .Include(u => u.Gender) // ✅ تحميل معلومات الجنس
                .ToList();

            // ✅ تجهيز البيانات مع تحويل رقم الجنس إلى نص
            var result = users.Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.NationalId,
                u.BirthDate,
                Gender = u.Gender?.Name ?? "غير محدد", // ✅ عرض اسم الجنس أو "غير محدد"
                u.ProfileImageUrl,
                u.IsProfileComplete
            });

            return Ok(result);
        }


        [HttpPut("update-password/{id}")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] string newPassword)
        {
            var adminEmail = Request.Headers["IsAdminEmail"].ToString();
            if (adminEmail != "ali.admin@trust.com")
                return Unauthorized(new { message = "غير مصرح لك" });

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "المستخدم غير موجود" });

            user.Password = newPassword;
            await _context.SaveChangesAsync();
            return Ok(new { message = "تم تحديث كلمة المرور" });
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var adminEmail = Request.Headers["IsAdminEmail"].ToString();
            if (adminEmail != "ali.admin@trust.com")
                return Unauthorized(new { message = "غير مصرح لك" });

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound(new { message = "المستخدم غير موجود" });

            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok(new { message = "تم حذف المستخدم" });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto updatedUser)
        {
            var adminEmail = Request.Headers["IsAdminEmail"].ToString();
            if (adminEmail != "ali.admin@trust.com")
                return Unauthorized(new { message = "غير مصرح لك" });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == updatedUser.Id);
            if (user == null) return NotFound(new { message = "المستخدم غير موجود" });

            user.FullName = updatedUser.FullName ?? user.FullName;

            // ✅ تحديث رقم الجنس فقط (GenderId)
            if (updatedUser.GenderId != 0)
                user.GenderId = updatedUser.GenderId;

            user.NationalId = updatedUser.NationalId ?? user.NationalId;
            user.Address = updatedUser.Address ?? user.Address;
            user.PhoneNumber = updatedUser.PhoneNumber ?? user.PhoneNumber;
            user.BirthDate = updatedUser.BirthDate ?? user.BirthDate;

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم تحديث الملف الشخصي بنجاح" });
        }


        [HttpPut("update-own")]
        public async Task<IActionResult> UpdateOwnProfile([FromBody] UserUpdateDto updatedUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == updatedUser.Id);
            if (user == null)
                return NotFound(new { message = "المستخدم غير موجود" });

            user.FullName = updatedUser.FullName ?? user.FullName;

            // ✅ تحديث رقم الجنس فقط (GenderId)
            if (updatedUser.GenderId != 0)
                user.GenderId = updatedUser.GenderId;

            user.NationalId = updatedUser.NationalId ?? user.NationalId;
            user.Address = updatedUser.Address ?? user.Address;
            user.PhoneNumber = updatedUser.PhoneNumber ?? user.PhoneNumber;
            user.BirthDate = updatedUser.BirthDate ?? user.BirthDate;

            await _context.SaveChangesAsync();
            var gender = await _context.Genders.FirstOrDefaultAsync(g => g.Id == user.GenderId);
            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.NationalId,
                user.BirthDate,
                GenderId = user.GenderId,
                Gender = gender?.Name, // ✅ أضفنا اسم الجنس
                user.ProfileImageUrl
            });

        }


        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageDto dto)
        {
            if (dto.Image == null || dto.Image.Length == 0)
                return BadRequest("لم يتم تحميل صورة");

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                return NotFound("المستخدم غير موجود");

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
            var savePath = Path.Combine("wwwroot", "uploads", fileName);

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            user.ProfileImageUrl = $"/uploads/{fileName}";
            await _context.SaveChangesAsync();

            return Ok(new { profileImageUrl = user.ProfileImageUrl });
        }
    }
}
