using CineGo.DTO.Common;
using CineGo.DTO.PromoCode;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineGo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class PromoCodeController : Controller
    {
        private readonly IPromoCodeService _promoService;

        public PromoCodeController(IPromoCodeService promoService)
        {
            _promoService = promoService;
        }

        // ------------------- Danh sách + tìm kiếm -------------------
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string searchCode = "")
        {
            ApiResponse response = await _promoService.GetPromoCodesPagedAsync(page, pageSize, searchCode);

            ViewBag.SearchCode = searchCode;

            if (response.Success && response.Data is PagedResult<PromoCodeDTO> pagedResult)
            {
                return View(pagedResult);
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Không thể tải danh sách mã giảm giá.";
                return View(new PagedResult<PromoCodeDTO>());
            }
        }

        // ------------------- Form thêm / sửa -------------------
        [HttpGet]
        public async Task<IActionResult> GetForm(int id = 0)
        {
            PromoCodeDTO model;

            if (id == 0)
            {
                model = new PromoCodeDTO
                {
                    ValidFrom = DateTime.Today,
                    ValidTo = DateTime.Today.AddDays(7),
                    DiscountType = "Percent",
                    IsActive = true
                };
            }
            else
            {
                var response = await _promoService.GetPromoCodeByIdAsync(id);

                if (!response.Success || response.Data == null)
                {
                    ModelState.AddModelError(string.Empty, response.Message ?? "Không tìm thấy mã giảm giá.");
                    return PartialView("_PromoCodeFormPartial", new PromoCodeDTO());
                }

                model = (PromoCodeDTO)response.Data;
            }

            return PartialView("_PromoCodeFormPartial", model);
        }

        // ------------------- Lưu -------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromForm] PromoCodeDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_PromoCodeFormPartial", dto);
            }

            ApiResponse response = dto.Id == 0
                ? await _promoService.AddPromoCodeAsync(dto)
                : await _promoService.UpdatePromoCodeAsync(dto.Id, dto);

            if (response.Success)
            {
                return Json(new { success = true, message = response.Message });
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "Có lỗi xảy ra.");
                return PartialView("_PromoCodeFormPartial", dto);
            }
        }

        // ------------------- Xóa -------------------
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _promoService.DeletePromoCodeAsync(id);

            return Json(new
            {
                success = response.Success,
                message = response.Message ?? (response.Success ? "Xóa thành công" : "Không thể xóa.")
            });
        }

        // ------------------- Tìm theo mã giảm giá -------------------
        [HttpGet]
        public async Task<IActionResult> GetByCode(string code)
        {
            var response = await _promoService.GetPromoCodeByCodeAsync(code);
            return Json(response);
        }

        // ------------------- Lọc theo loại giảm giá -------------------
        [HttpGet]
        public async Task<IActionResult> FilterByType(string discountType)
        {
            var response = await _promoService.GetPromoCodesByTypeAsync(discountType);
            return Json(response);
        }
    }
}
