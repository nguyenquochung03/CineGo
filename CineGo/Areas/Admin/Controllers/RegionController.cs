using CineGo.DTO;
using CineGo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CineGo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class RegionController : Controller
    {
        private readonly IRegionService _regionService;

        public RegionController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        // GET: /Admin/Region/Index
        public async Task<IActionResult> Index(int page = 1)
        {
            var response = await _regionService.GetPagedAsync(page, 5);
            return PartialView("_List", response.Data);
        }

        // GET: /Admin/Region/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var response = await _regionService.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(response.Message);

            return PartialView("_List", response.Data);
        }

        // POST: /Admin/Region/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _regionService.CreateAsync(dto);
            return Json(response);
        }

        // POST: /Admin/Region/Update/5
        [HttpPost("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RegionDTO dto)
        {
            Console.WriteLine($"Received DTO: {dto?.Name}");
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToArray();

                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            var response = await _regionService.UpdateAsync(id, dto);
            return Json(response);
        }

        // POST: /Admin/Region/Delete/5
        [HttpPost("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _regionService.DeleteAsync(id);
            return Json(response);
        }
    }
}
