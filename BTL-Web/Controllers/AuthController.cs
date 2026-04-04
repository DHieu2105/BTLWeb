
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BTL_Web.Constants;
using BTL_Web.Services;
using BTL_Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IUserInfoService _userInfoService;

    public AuthController(IAuthService authService, IUserInfoService userInfoService)
    {
        _authService = authService;
        _userInfoService = userInfoService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated ?? false)
            return RedirectToAction("Index", "Home");
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Vui lòng nhập tên đăng nhập và mật khẩu";
            return View();
        }

        var account = await _authService.AuthenticateAsync(username, password);
        
        if (account == null)
        {
            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
            return View();
        }

        // Lấy tên người dùng
        var fullName = await _userInfoService.GetUserFullNameAsync(username, account.Role ?? "");
        
        // Tạo claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, account.Username),
            new Claim(ClaimTypes.Name, fullName ?? account.Username),
            new Claim(ClaimTypes.Role, account.Role ?? ""),
            new Claim("Username", account.Username),
            new Claim("Role", account.Role ?? "")
        };

        // Thêm thông tin dựa vào role
        if (!string.IsNullOrEmpty(account.MaNv))
            claims.Add(new Claim("MaNV", account.MaNv));
        
        if (!string.IsNullOrEmpty(account.MaGv))
            claims.Add(new Claim("MaGV", account.MaGv));
        
        if (!string.IsNullOrEmpty(account.MaHocVien))
            claims.Add(new Claim("MaHocVien", account.MaHocVien));

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Sign in
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            });

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return RedirectToAction("Login");

        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            return RedirectToAction("Login");

        var userInfo = await _userInfoService.GetUserInfoAsync(username, role);
        
        return View(new { Account = userInfo, RoleDisplay = AppRoles.GetRoleDisplayName(role) });
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return RedirectToAction("Login");

        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            return RedirectToAction("Login");

        var userInfo = await _userInfoService.GetUserInfoAsync(username, role);
        
        ViewBag.Account = userInfo;
        ViewBag.RoleDisplay = AppRoles.GetRoleDisplayName(role);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return RedirectToAction("Login");

        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            return RedirectToAction("Login");

        // Validate input
        if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
        {
            TempData["Error"] = "Vui lòng điền đủ thông tin";
            return RedirectToAction("EditProfile");
        }

        if (newPassword != confirmPassword)
        {
            TempData["Error"] = "Mật khẩu mới và xác nhận không khớp";
            return RedirectToAction("EditProfile");
        }

        if (newPassword.Length < 6)
        {
            TempData["Error"] = "Mật khẩu mới phải ít nhất 6 ký tự";
            return RedirectToAction("EditProfile");
        }

        var result = await _authService.ChangePasswordAsync(username, oldPassword, newPassword);

        if (!result)
        {
            TempData["Error"] = "Mật khẩu cũ không đúng";
            return RedirectToAction("EditProfile");
        }

        TempData["Success"] = "Đổi mật khẩu thành công";
        return RedirectToAction("EditProfile");
    }

    // DEBUG: Setup Admin Account
    [HttpGet("auth/setup")]
    public IActionResult Setup()
    {
        return Content($@"
            <h2>Setup Tài Khoản Admin</h2>
            <p><a href='/auth/setup-create'>Click để tạo tài khoản admin</a></p>
        ", "text/html");
    }

    [HttpGet("auth/setup-create")]
    public async Task<IActionResult> SetupCreate()
    {
        try
        {
            var context = HttpContext.RequestServices.GetRequiredService<TtanContext>();

            // Check if admin exists
            var admin = await context.TaiKhoans.FirstOrDefaultAsync(x => x.Username == "admin");
            
            if (admin != null)
            {
                // Update password
                admin.Password = "123";
                admin.Role = "Admin";
                context.TaiKhoans.Update(admin);
            }
            else
            {
                // Create new admin
                var newAdmin = new TaiKhoan
                {
                    Username = "admin",
                    Password = "123",
                    Role = "Admin",
                    MaNv = null,
                    MaGv = null,
                    MaHocVien = null
                };
                context.TaiKhoans.Add(newAdmin);
            }

            await context.SaveChangesAsync();

            return Content($@"
                <h2>✅ Thành Công!</h2>
                <p>Tài khoản Admin đã được tạo/cập nhật.</p>
                <p><strong>Username:</strong> admin</p>
                <p><strong>Password:</strong> 123</p>
                <p><a href='/auth/login'>Vào trang login</a></p>
            ", "text/html");
        }
        catch (Exception ex)
        {
            return Content($@"
                <h2>❌ Lỗi!</h2>
                <p>{ex.Message}</p>
                <p>{ex.InnerException?.Message}</p>
            ", "text/html");
        }
    }
}