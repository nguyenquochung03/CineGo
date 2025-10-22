using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CineGo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly string _jwtSecret;

        public DashboardController(IConfiguration configuration)
        {
            _jwtSecret = configuration["Jwt:Key"]
                ?? throw new ArgumentNullException("JWT secret key chưa được cấu hình.");
        }

        public IActionResult Index()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
            {
                // Chưa đăng nhập → redirect login
                return RedirectToAction("Login", "AdminAuth", new { area = "Admin" });
            }

            var jwtHandler = new JwtSecurityTokenHandler();
            try
            {
                jwtHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true, // kiểm tra token hết hạn
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecret)),
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Token hợp lệ → cho phép vào Dashboard
                return View();
            }
            catch
            {
                // Token hết hạn hoặc không hợp lệ → xóa cookie, redirect login với expired
                Response.Cookies.Delete("jwt");
                return RedirectToAction("Login", "AdminAuth", new { area = "Admin", expired = true });
            }
        }
    }
}
