using CineGo.DTO;
using CineGo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CineGo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class CinemaController : Controller
    {
        private readonly ICinemaService _cinemaService;

        public CinemaController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        // GET: /Admin/Cinema/Index
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int? cityId = null)
        {
            var response = await _cinemaService.GetPagedAsync(page, 5, cityId);
            return PartialView("_List", response.Data);
        }

        // GET: /Admin/Cinema/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _cinemaService.GetByIdAsync(id);
            if (!response.Success) return NotFound(response.Message);
            return PartialView("_List", response.Data);
        }

        // POST: /Admin/Cinema/Create
        [HttpPost]
        public async Task<JsonResult> Create([FromBody] CinemaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            var response = await _cinemaService.CreateAsync(dto);
            if (!response.Success)
                return Json(new { success = false, message = response.Message });

            return Json(new { success = true, message = response.Message });
        }

        // POST: /Admin/Cinema/Update/5
        [HttpPost("{id}")]
        public async Task<JsonResult> Update(int id, [FromBody] CinemaDTO dto)
        {
            Console.WriteLine(dto.Amenities);
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            var response = await _cinemaService.UpdateAsync(id, dto);
            if (!response.Success)
                return Json(new { success = false, message = response.Message });

            return Json(new { success = true, message = response.Message });
        }

        // POST: /Admin/Cinema/Delete/5
        [HttpPost("{id}")]
        public async Task<JsonResult> Delete(int id)
        {
            var response = await _cinemaService.DeleteAsync(id);
            if (!response.Success)
                return Json(new { success = false, message = response.Message });

            return Json(new { success = true, message = response.Message });
        }

        // API / Ajax: lấy danh sách cinema theo city
        [HttpGet]
        public async Task<JsonResult> GetByCity(int cityId)
        {
            var response = await _cinemaService.GetPagedAsync(1, 100, cityId);
            return Json(response);
        }
    }
}
