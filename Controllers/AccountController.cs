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

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Message"] = "You have successfully logged out.";
        return RedirectToAction("Index", "Home");
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
