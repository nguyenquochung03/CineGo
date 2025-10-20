using CineGo.DTO.Common;
using CineGo.DTO.ShowtimePrice;
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
    public class ShowtimePriceController : Controller
    {
        private readonly IShowtimePriceService _service;

        public ShowtimePriceController(IShowtimePriceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetByShowtime(int showtimeId, int page = 1, int pageSize = 5)
        {
            var response = await _service.GetByShowtimeAsync(showtimeId, page, pageSize);

            // Lấy Items từ PagedResult
            var pagedResult = response.Data as PagedResult<ShowtimePriceDTO> ?? new PagedResult<ShowtimePriceDTO>();

            ViewBag.ShowtimeId = showtimeId;
            return PartialView("_ShowtimePriceList", pagedResult);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShowtimePriceCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return Json(ApiResponse.ErrorResponse(400, "Dữ liệu không hợp lệ."));

            var response = await _service.CreateAsync(dto);
            return Json(response);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ShowtimePriceUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return Json(ApiResponse.ErrorResponse(400, "Dữ liệu không hợp lệ."));

            var response = await _service.UpdateAsync(dto);
            return Json(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _service.DeleteAsync(id);
            return Json(response);
        }
    }
}
