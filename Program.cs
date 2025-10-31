using TrustInsuranceApi1.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ✅ ضبط المنفذ ليتوافق مع Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ✅ إعداد الاتصال بقاعدة البيانات (SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ إعداد التحكم في تحويل JSON وتفادي المشاكل الدائرية
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// ✅ إعداد CORS للسماح لفرونت إند React بالوصول من Vercel و localhost
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                "https://trust-frontend-2l4n.vercel.app", // الرابط على Vercel
                "http://localhost:3000"                     // نسخة التطوير المحلية
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ✅ تفعيل Swagger للتوثيق دائماً (Development + Production)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// تمكين Swagger لجميع البيئات
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trust API V1");
    c.RoutePrefix = string.Empty; // هذا يجعل Swagger في صفحة الجذر '/'
});


// ✅ ترتيب الـ Middleware
app.UseStaticFiles(); // لرفع أي ملفات ثابتة مثل الصور
app.UseRouting();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

// ✅ تشغيل التطبيق مع التعامل مع أي استثناء عند التشغيل
try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("🔥🔥🔥 حدث خطأ أثناء تشغيل التطبيق:");
    Console.WriteLine(ex.ToString());
}
