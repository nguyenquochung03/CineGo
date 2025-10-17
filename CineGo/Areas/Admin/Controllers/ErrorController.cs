using Microsoft.AspNetCore.Mvc;

namespace CineGo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ErrorController : Controller
    {
        [Route("Admin/Error/{statusCode?}")]
        public IActionResult AdminError(int statusCode = 500)
        {
            ViewData["StatusCode"] = statusCode;
            ViewData["Title"] = statusCode switch
            {
                404 => "Không tìm thấy trang quản trị",
                403 => "Không có quyền truy cập khu vực quản trị",
                500 => "Lỗi hệ thống quản trị",
                _ => "Lỗi không xác định"
            };

            return View();
        }
    }
}
