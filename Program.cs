using TrustInsuranceApi1.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

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

// ✅ إعداد CORS للسماح لفرونت إند React بالوصول من Vercel
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("https://trust-frontend-2l4n.vercel.app") // رابط الريأكت على Vercel
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ✅ تفعيل Swagger للتوثيق
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ تمكين الأدوات فقط في وضع التطوير
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
