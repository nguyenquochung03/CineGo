using CineGo.DTO.PricingRule;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineGo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class PricingRuleController : Controller
    {
        private readonly IPricingRuleService _pricingRuleService;

        public PricingRuleController(IPricingRuleService pricingRuleService)
        {
            _pricingRuleService = pricingRuleService;
        }

        // GET: /Admin/PricingRule/
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Admin/PricingRule/List
        [HttpGet]
        public async Task<IActionResult> List(int page = 1, int pageSize = 5)
        {
            var response = await _pricingRuleService.GetAllAsync(page, pageSize);
            if (!response.Success) return Json(response);
            return PartialView("_PricingRuleList", response.Data);
        }

        // POST: /Admin/PricingRule/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PricingRuleCreateDTO dto)
        {
            Console.WriteLine(dto);
            if (!ModelState.IsValid)
                return Json(ApiResponse.ErrorResponse(400, "Dữ liệu không hợp lệ."));
            Console.WriteLine(dto);
            var response = await _pricingRuleService.CreateAsync(dto);
            return Json(response);
        }

        // POST: /Admin/PricingRule/Update/{id}
        [HttpPost("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PricingRuleUpdateDTO dto)
        {
            if (id != dto.Id)
                return Json(ApiResponse.ErrorResponse(400, "ID không khớp."));

            if (!ModelState.IsValid)
                return Json(ApiResponse.ErrorResponse(400, "Dữ liệu không hợp lệ."));

            var response = await _pricingRuleService.UpdateAsync(dto);
            return Json(response);
        }

        // POST: /Admin/PricingRule/Delete/{id}
        [HttpPost("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _pricingRuleService.DeleteAsync(id);
            return Json(response);
        }
    }
}