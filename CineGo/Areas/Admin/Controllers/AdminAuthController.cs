using CineGo.DTO.User;
using CineGo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CineGo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    public class AdminAuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly string _jwtSecret;

        public AdminAuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _jwtSecret = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Khóa bí mật JWT chưa được cấu hình.");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            var token = HttpContext.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                try
                {
                    var key = Encoding.ASCII.GetBytes(_jwtSecret);
                    jwtHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true, // kiểm tra token hết hạn
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    // Token hợp lệ → admin đã đăng nhập → redirect Dashboard
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                catch
                {
                    // Token hết hạn hoặc không hợp lệ → xóa cookie
                    Response.Cookies.Delete("jwt");
                }
            }

            // Nếu có query expired → hiển thị alert
            if (HttpContext.Request.Query["expired"] == "true")
            {
                ViewBag.AlertMessage = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.";
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            // Gọi service để xác thực và tạo token
            var result = await _authService.AuthenticateAdminAsync(loginDto, _jwtSecret);

            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                return View();
            }

            // Kiểm tra Data null trước khi dùng
            if (result.Data is not null)
            {
                // Dùng dynamic an toàn hơn
                var data = result.Data as dynamic;

                string? token = data?.Token;
                UserResponseDTO? admin = data?.User as UserResponseDTO;

                if (string.IsNullOrEmpty(token) || admin is null)
                {
                    ViewBag.Error = "Thông tin đăng nhập không hợp lệ.";
                    return View();
                }

                // Tạo cookie HttpOnly
                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(2)
                });

                // Lưu thông tin admin
                HttpContext.Session.SetInt32("AdminId", admin.Id);
                HttpContext.Session.SetString("AdminName", admin.Name);

                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            ViewBag.Error = "Dữ liệu đăng nhập trống.";
            return View();
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
