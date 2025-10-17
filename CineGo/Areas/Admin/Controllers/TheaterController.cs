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
    public class TheaterController : Controller
    {
        private readonly ITheaterService _theaterService;

        public TheaterController(ITheaterService theaterService)
        {
            _theaterService = theaterService;
        }

        // GET: /Admin/Theater/Index?page=1&cinemaId=5
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int? cinemaId = null)
        {
            var response = await _theaterService.GetPagedAsync(page, 5, cinemaId);
            return PartialView("_List", response.Data);
        }

        // GET: /Admin/Theater/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _theaterService.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(response.Message);

            return PartialView("_List", response.Data);
        }

        // POST: /Admin/Theater/Create
        [HttpPost]
        public async Task<JsonResult> Create([FromBody] TheaterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            var response = await _theaterService.CreateAsync(dto);
            if (!response.Success)
                return Json(new { success = false, message = response.Message });

            return Json(new { success = true, message = response.Message });
        }

        // POST: /Admin/Theater/Update/5
        [HttpPost("{id}")]
        public async Task<JsonResult> Update(int id, [FromBody] TheaterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            var response = await _theaterService.UpdateAsync(id, dto);
            if (!response.Success)
                return Json(new { success = false, message = response.Message });

            return Json(new { success = true, message = response.Message });
        }

        // POST: /Admin/Theater/Delete/5
        [HttpPost("{id}")]
        public async Task<JsonResult> Delete(int id)
        {
            var response = await _theaterService.DeleteAsync(id);
            if (!response.Success)
                return Json(new { success = false, message = response.Message });

            return Json(new { success = true, message = response.Message });
        }

        // API / Ajax: lấy danh sách phòng chiếu theo cinema
        [HttpGet]
        public async Task<JsonResult> GetByCinema(int cinemaId)
        {
            var response = await _theaterService.GetPagedAsync(1, 100, cinemaId);
            return Json(response);
        }
    }
}
