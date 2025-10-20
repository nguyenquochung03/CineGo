using CineGo.DTO.Common;
using CineGo.DTO.Showtime;
using CineGo.DTO.ShowTime;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CineGo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class ShowtimeController : Controller
    {
        private readonly IShowtimeService _showtimeService;

        public ShowtimeController(IShowtimeService showtimeService)
        {
            _showtimeService = showtimeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> List(int page = 1, int pageSize = 5)
        {
            var response = await _showtimeService.GetAllAsync(page, pageSize);
            return PartialView("_ShowtimeList", response.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _showtimeService.GetByIdAsync(id);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShowtimeCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return Json(ApiResponse.ErrorResponse(400, "Dữ liệu không hợp lệ."));

            var response = await _showtimeService.CreateAsync(dto);
            return Json(response);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ShowtimeUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return Json(ApiResponse.ErrorResponse(400, "Dữ liệu không hợp lệ."));

            var response = await _showtimeService.UpdateAsync(dto);
            return Json(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _showtimeService.DeleteAsync(id);
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetByDate(DateTime date)
        {
            var response = await _showtimeService.GetByDateAsync(date);

            var items = response.Data as List<ShowtimeDTO>;
            if (items == null) items = new List<ShowtimeDTO>();

            var pagedResult = new PagedResult<ShowtimeDTO>
            {
                Items = items,
                Page = 1,
                TotalPages = 1,
                TotalItems = items.Count
            };

            return PartialView("_ShowtimeList", pagedResult);
        }

        [HttpGet]
        public async Task<IActionResult> GetByDateTimeRange(DateTime date, TimeSpan start, TimeSpan end)
        {
            var response = await _showtimeService.GetByDateTimeRangeAsync(date, start, end);

            var items = response.Data as List<ShowtimeDTO>;
            if (items == null) items = new List<ShowtimeDTO>();

            var pagedResult = new PagedResult<ShowtimeDTO>
            {
                Items = items,
                Page = 1,
                TotalPages = 1,
                TotalItems = items.Count
            };

            return PartialView("_ShowtimeList", pagedResult);
        }

        [HttpGet]
        public async Task<IActionResult> GetByMovieName(string movieName)
        {
            var response = await _showtimeService.GetByMovieNameAsync(movieName);

            var items = response.Data as List<ShowtimeDTO>;
            if (items == null) items = new List<ShowtimeDTO>();

            var pagedResult = new PagedResult<ShowtimeDTO>
            {
                Items = items,
                Page = 1,
                TotalPages = 1,
                TotalItems = items.Count
            };

            return PartialView("_ShowtimeList", pagedResult);
        }

    }
}
