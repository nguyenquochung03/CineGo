using CineGo.DTO.PricingDetail;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CineGo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class PricingDetailController : Controller
    {
        private readonly IPricingDetailService _pricingDetailService;

        public PricingDetailController(IPricingDetailService pricingDetailService)
        {
            _pricingDetailService = pricingDetailService;
        }

        // GET: /Admin/PricingDetail/List
        [HttpGet]
        public async Task<IActionResult> List(int ruleId, int page = 1, int pageSize = 5)
        {
            if (ruleId <= 0)
                return Json(ApiResponse.ErrorResponse(400, "Thiếu ID của quy tắc giá."));

            var response = await _pricingDetailService.GetByRuleIdAsync(ruleId, page, pageSize);
            if (!response.Success) return Json(response);

            ViewBag.PricingRuleId = ruleId;
            return PartialView("_PricingDetailList", response.Data);
        }

        // POST: /Admin/PricingDetail/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PricingDetailCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return Json(ApiResponse.ErrorResponse(400, "Dữ liệu không hợp lệ."));

            var response = await _pricingDetailService.CreateAsync(dto);
            return Json(response);
        }

        // POST: /Admin/PricingDetail/Update/{id}
        [HttpPost("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PricingDetailUpdateDTO dto)
        {
            if (id != dto.Id)
                return Json(ApiResponse.ErrorResponse(400, "ID không khớp."));

            if (!ModelState.IsValid)
                return Json(ApiResponse.ErrorResponse(400, "Dữ liệu không hợp lệ."));

            var response = await _pricingDetailService.UpdateAsync(dto);
            return Json(response);
        }

        // POST: /Admin/PricingDetail/Delete/{id}
        [HttpPost("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _pricingDetailService.DeleteAsync(id);
            return Json(response);
        }
    }
}