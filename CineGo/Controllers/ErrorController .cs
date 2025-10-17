using Microsoft.AspNetCore.Mvc;

namespace CineGo.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewData["Title"] = "Không tìm thấy trang";
                    ViewData["Message"] = "Trang bạn yêu cầu không tồn tại hoặc đã bị xóa.";
                    break;

                case 403:
                    ViewData["Title"] = "Truy cập bị từ chối";
                    ViewData["Message"] = "Bạn không có quyền truy cập vào trang này.";
                    break;

                case 500:
                    ViewData["Title"] = "Lỗi hệ thống";
                    ViewData["Message"] = "Đã xảy ra lỗi nội bộ. Vui lòng thử lại sau.";
                    break;

                default:
                    ViewData["Title"] = "Lỗi không xác định";
                    ViewData["Message"] = "Đã xảy ra lỗi. Vui lòng quay lại trang chủ.";
                    break;
            }

            return View("CustomError");
        }
    }
}
