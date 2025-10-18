using CineGo.DTO.PricingRuleDay;
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
    public class PricingRuleDayController : Controller
    {
        private readonly IPricingRuleDayService _pricingRuleDayService;

        public PricingRuleDayController(IPricingRuleDayService pricingRuleDayService)
        {
            _pricingRuleDayService = pricingRuleDayService;
        }

        // GET: /Admin/PricingRuleDay/List
        [HttpGet]
        public async Task<IActionResult> List(int ruleId, int page = 1, int pageSize = 10)
        {
            if (ruleId <= 0)
                return Json(ApiResponse.ErrorResponse(400, "Thiếu ID của quy tắc giá."));

            var response = await _pricingRuleDayService.GetByRuleIdAsync(ruleId, page, pageSize);
            if (!response.Success) return Json(response);

            ViewBag.PricingRuleId = ruleId;
            return PartialView("_PricingRuleDayList", response.Data);
        }

        // POST: /Admin/PricingRuleDay/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PricingRuleDayCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return Json(ApiResponse.ErrorResponse(400, "Dữ liệu không hợp lệ."));

            var response = await _pricingRuleDayService.CreateAsync(dto);
            return Json(response);
        }

        // POST: /Admin/PricingRuleDay/Update/{id}
        [HttpPost("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PricingRuleDayUpdateDTO dto)
        {
            if (id != dto.Id)
                return Json(ApiResponse.ErrorResponse(400, "ID không khớp."));

            if (!ModelState.IsValid)
                return Json(ApiResponse.ErrorResponse(400, "Dữ liệu không hợp lệ."));

            var response = await _pricingRuleDayService.UpdateAsync(dto);
            return Json(response);
        }

        // POST: /Admin/PricingRuleDay/Delete/{id}
        [HttpPost("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _pricingRuleDayService.DeleteAsync(id);
            return Json(response);
        }
    }
}