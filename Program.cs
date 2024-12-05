using Microsoft.EntityFrameworkCore;
using PakinProject.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using QuestPDF.Infrastructure; // Import สำหรับการตั้งค่า License


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// เพิ่มการตั้งค่า DbContext และการเชื่อมต่อฐานข้อมูล
builder.Services.AddDbContext<PakinProjectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ตั้งค่าการรับรองความถูกต้อง
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied"; // เพิ่มหน้ากรณีไม่มีสิทธิ์
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware สำหรับการรับรองความถูกต้อง
app.UseAuthentication();
app.UseAuthorization();

// ตั้งค่า License สำหรับ QuestPDF
QuestPDF.Settings.License = LicenseType.Community; // ใช้ License แบบ Community

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
