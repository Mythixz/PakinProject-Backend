using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PakinProject.Data;
using PakinProject.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class AccountController : Controller
{
    private readonly PakinProjectContext _context;
    private const string PredefinedOtp = "123456"; // กำหนด OTP สำหรับทดสอบ

    public AccountController(PakinProjectContext context)
    {
        _context = context;
    }

    // ฟังก์ชันสำหรับหน้า Login (GET)
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // ฟังก์ชันสำหรับหน้า Login (POST)
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user != null && VerifyPassword(model.Password, user.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role) // ใช้ Role จากฐานข้อมูล
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                TempData["Message"] = $"Welcome back, {user.Username}!";
                return RedirectToAction("Index", user.Role == "Admin" ? "Admin" : "Store");
            }

            ViewBag.ErrorMessage = "Invalid email or password. Please try again.";
        }
        else
        {
            ViewBag.ErrorMessage = "Please fill out the form correctly.";
        }

        return View(model);
    }

    // ฟังก์ชันสำหรับหน้า Register (GET)
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // ฟังก์ชันสำหรับหน้า Register (POST)
    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("", "Email is already registered.");
                return View(model);
            }

            var user = new User
            {
                Username = model.Email.Split('@')[0],
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                Role = "User" // ค่าเริ่มต้น Role เป็น User
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var userWallet = new UserWallet
            {
                UserId = user.Id,
                Balance = 1000m // ค่าเริ่มต้น Wallet
            };

            _context.UserWallets.Add(userWallet);
            _context.SaveChanges();

            TempData["Message"] = "Registration successful. Please log in.";
            return RedirectToAction("Login");
        }

        return View(model);
    }

    // ฟังก์ชันสำหรับการออกจากระบบ (Logout)
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Message"] = "You have successfully logged out.";
        return RedirectToAction("Index", "Home");
    }

    // ฟังก์ชันสำหรับหน้า Forgot Password (GET)
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    // ฟังก์ชันสำหรับหน้า Forgot Password (POST)
    [HttpPost]
    public IActionResult ForgotPassword(string email)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user != null)
        {
            TempData["Email"] = email; // เก็บอีเมลไว้ชั่วคราว
            return RedirectToAction("EnterOtp");
        }

        ModelState.AddModelError("", "Email not found.");
        return View();
    }

    // ฟังก์ชันสำหรับหน้า Enter OTP (GET)
    [HttpGet]
    public IActionResult EnterOtp()
    {
        return View();
    }

    // ฟังก์ชันสำหรับหน้า Enter OTP (POST)
    [HttpPost]
    public IActionResult EnterOtp(string otp)
    {
        if (otp == PredefinedOtp) // ตรวจสอบ OTP
        {
            return RedirectToAction("ResetPassword");
        }

        ViewBag.ErrorMessage = "Invalid OTP. Please try again.";
        return View();
    }

    // ฟังก์ชันสำหรับหน้า Reset Password (GET)
    [HttpGet]
    public IActionResult ResetPassword()
    {
        return View();
    }

    // ฟังก์ชันสำหรับหน้า Reset Password (POST)
    [HttpPost]
    public IActionResult ResetPassword(string newPassword)
    {
        var email = TempData["Email"]?.ToString();
        if (!string.IsNullOrEmpty(email))
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.PasswordHash = HashPassword(newPassword);
                _context.SaveChanges();

                TempData["Message"] = "Password reset successful. Please log in.";
                return RedirectToAction("Login");
            }
        }

        ModelState.AddModelError("", "Something went wrong. Please try again.");
        return View();
    }

    // ฟังก์ชันสำหรับ Hash รหัสผ่าน
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    // ฟังก์ชันสำหรับตรวจสอบรหัสผ่านที่ป้อน
    private bool VerifyPassword(string password, string hashedPassword)
    {
        return HashPassword(password) == hashedPassword;
    }
}
