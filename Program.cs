using Microsoft.EntityFrameworkCore;
using PakinProject.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using PakinProject.Filters; // เพิ่ม Namespace สำหรับ Filter

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // เพิ่ม Global Filter สำหรับ WalletBalanceFilter
    options.Filters.Add<WalletBalanceFilter>();
});
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();


// เพิ่มการตั้งค่า DbContext และการเชื่อมต่อฐานข้อมูล
builder.Services.AddDbContext<PakinProjectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// เพิ่มการตั้งค่าการรับรองความถูกต้อง
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied"; // เพิ่มหน้ากรณีไม่มีสิทธิ์
    });

// เพิ่ม WalletBalanceFilter เป็น Dependency
builder.Services.AddScoped<WalletBalanceFilter>();

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

// เพิ่ม middleware สำหรับการรับรองความถูกต้อง
app.UseAuthentication();
app.UseAuthorization();

// เพิ่ม Route สำหรับ Admin และ User
app.MapControllerRoute(
    name: "Admin", 
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");  // เส้นทางสำหรับ Admin

app.MapControllerRoute(
    name: "Order",
    pattern: "Order/{controller=Order}/{action=Index}/{id?}"); // เส้นทางสำหรับ Order

app.MapControllerRoute(
    name: "Store",
    pattern: "Store/{controller=Store}/{action=Index}/{id?}"); // เส้นทางสำหรับ Store

app.MapControllerRoute(
    name: "default", 
    pattern: "{controller=Home}/{action=Index}/{id?}");  // เส้นทางพื้นฐาน
app.Run();
