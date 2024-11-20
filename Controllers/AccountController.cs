using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // เขียนโค้ดเพื่อยืนยันตัวตน
            return RedirectToAction("Index", "Home");
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
            // เขียนโค้ดเพื่อสมัครสมาชิก
            return RedirectToAction("Index", "Home");
        }
        return View(model);
    }
}
