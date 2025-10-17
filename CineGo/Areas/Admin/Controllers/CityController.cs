using CineGo.DTO;
using CineGo.Models;
using CineGo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CineGo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class CityController : Controller
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        // GET: /Admin/City/Index
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int? regionId = null)
        {
            var response = await _cityService.GetPagedAsync(page, 5, regionId);
            return PartialView("_List", response.Data);
        }

        // GET: /Admin/City/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _cityService.GetByIdAsync(id);
            if (!response.Success) return NotFound(response.Message);
            return PartialView("_List", response.Data);
        }

        // POST: /Admin/City/Create
        [HttpPost]
        public async Task<JsonResult> Create([FromBody] CityDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToArray();
                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            var response = await _cityService.CreateAsync(dto);
            if (!response.Success)
                return Json(new { success = false, message = response.Message });

            return Json(new { success = true, message = response.Message });
        }

        // POST: /Admin/City/Update/5
        [HttpPost("{id}")]
        public async Task<JsonResult> Update(int id, [FromBody] CityDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToArray();
                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            var response = await _cityService.UpdateAsync(id, dto);
            if (!response.Success)
                return Json(new { success = false, message = response.Message });

            return Json(new { success = true, message = response.Message });
        }

        // POST: /Admin/City/Delete/5
        [HttpPost("{id}")]
        public async Task<JsonResult> Delete(int id)
        {
            var response = await _cityService.DeleteAsync(id);
            if (!response.Success)
                return Json(new { success = false, message = response.Message });

            return Json(new { success = true, message = response.Message });
        }

        // API / Ajax: lấy danh sách city theo region
        [HttpGet]
        public async Task<JsonResult> GetByRegion(int regionId)
        {
            var response = await _cityService.GetPagedAsync(1, 100, regionId);
            return Json(response);
        }
    }
}
