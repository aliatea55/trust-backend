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
                "https://trust-frontend-2l4n.vercel.app",
                "http://localhost:3000"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ✅ تفعيل Swagger للتوثيق
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ تمكين Swagger دائماً
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trust API V1");
    c.RoutePrefix = ""; // تجعل Swagger هو الصفحة الرئيسية
});

// ✅ ترتيب الـ Middleware
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

// ✅ إعادة توجيه أي طلب على "/" إلى Swagger UI (إذا RoutePrefix لم يكن "")
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

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
