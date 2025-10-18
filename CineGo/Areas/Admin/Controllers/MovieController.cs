using CineGo.DTO.Common;
using CineGo.DTO.Movie;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineGo.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTitle = "")
        {
            ApiResponse response;

            if (!string.IsNullOrWhiteSpace(searchTitle))
            {
                response = await _movieService.SearchByTitleAsync(searchTitle, page, pageSize);
                ViewBag.SearchTitle = searchTitle;
            }
            else
            {
                response = await _movieService.GetPagedAsync(page, pageSize);
            }

            if (response.Success && response.Data is PagedResult<MovieDTO> pagedResult)
            {
                return View(pagedResult);
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Không thể tải danh sách phim.";
                return View(new PagedResult<MovieDTO>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetForm(int id = 0)
        {
            MovieCreateUpdateDTO model;

            if (id == 0)
            {
                model = new MovieCreateUpdateDTO
                {
                    ReleaseDate = DateTime.Today
                };
            }
            else
            {
                var response = await _movieService.GetByIdAsync(id);
                if (!response.Success || response.Data == null)
                {
                    ModelState.AddModelError(string.Empty, response.Message ?? "Phim không tồn tại.");
                    return PartialView("_MovieFormPartial", new MovieCreateUpdateDTO());
                }

                if (response.Data is MovieDTO movieDto)
                {
                    model = new MovieCreateUpdateDTO
                    {
                        Id = movieDto.Id,
                        Title = movieDto.Title,
                        Slug = movieDto.Slug,
                        Runtime = movieDto.Runtime,
                        Rating = movieDto.Rating,
                        AgeLimit = movieDto.AgeLimit,
                        ReleaseDate = movieDto.ReleaseDate,
                        Synopsis = movieDto.Synopsis,
                        TrailerUrl = movieDto.TrailerUrl,
                        PosterUrl = movieDto.Posters?.FirstOrDefault()?.Url
                    };
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Dữ liệu phim không đúng định dạng.");
                    return PartialView("_MovieFormPartial", new MovieCreateUpdateDTO());
                }
            }

            return PartialView("_MovieFormPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromForm] MovieCreateUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_MovieFormPartial", dto);
            }

            var response = dto.Id == 0
                ? await _movieService.CreateAsync(dto)
                : await _movieService.UpdateAsync(dto.Id, dto);

            if (response.Success)
            {
                return Json(new { success = true, message = response.Message });
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "Có lỗi xảy ra.");
                return PartialView("_MovieFormPartial", dto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _movieService.DeleteAsync(id);

            if (response.Success)
            {
                return Json(new { success = true, message = response.Message });
            }
            else
            {
                return Json(new { success = false, message = response.Message ?? "Không thể xóa phim." });
            }
        }
    }
}