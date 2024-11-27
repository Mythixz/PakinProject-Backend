using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PakinProject.Data;
using PakinProject.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


public class AccountController : Controller
{
    private readonly PakinProjectContext _context;
    private const string PredefinedOtp = "123456"; // กำหนดค่า OTP คงที่สำหรับทดสอบ

    public AccountController(PakinProjectContext context)
    {
        _context = context; // Inject DbContext
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

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
                new Claim("UserId", user.Id.ToString()) // เพิ่ม UserId เป็น Claim
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                TempData["Message"] = $"Welcome back, {user.Username}!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Invalid email or password. Please try again.";
        }
        else
        {
            ViewBag.ErrorMessage = "Please fill out the form correctly.";
        }

        return View(model);
    }




    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // ตรวจสอบว่าอีเมลซ้ำหรือไม่
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("", "Email is already registered.");
                return View(model);
            }

            // สร้าง User ใหม่
            var user = new User
            {
                Username = model.Email.Split('@')[0],
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
            };

            // เพิ่ม User ลงในฐานข้อมูล
            _context.Users.Add(user);
            _context.SaveChanges(); // บันทึกเพื่อให้มี User ID กลับมา

            // สร้าง UserWallet สำหรับ User ใหม่
            var userWallet = new UserWallet
            {
                UserId = user.Id,  // ใช้ UserId ที่เพิ่งสร้าง
                Balance = 100000m     // กำหนดค่า Balance เป็น 0
            };

            // เพิ่ม UserWallet ลงในฐานข้อมูล
            _context.UserWallets.Add(userWallet);
            _context.SaveChanges(); // บันทึกข้อมูล UserWallet

            TempData["Message"] = "Registration successful. Please log in.";
            return RedirectToAction("Login");
        }

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        // ใช้ SignOutAsync เพื่อออกจากระบบ
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Message"] = "You have successfully logged out.";
        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

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

    [HttpGet]
    public IActionResult EnterOtp()
    {
        return View();
    }

    [HttpPost]
    public IActionResult EnterOtp(string otp)
    {
        if (otp == PredefinedOtp)
        {
            return RedirectToAction("ResetPassword");
        }

        ViewBag.ErrorMessage = "Invalid OTP. Please try again.";
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword()
    {
        return View();
    }

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

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        return HashPassword(password) == hashedPassword;
    }
}
