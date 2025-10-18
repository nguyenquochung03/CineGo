using CineGo.DTO;
using CineGo.DTO.Seat;
using CineGo.Services;
using CineGo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineGo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class SeatController : Controller
    {
        private readonly ISeatService _seatService;
        private readonly ITheaterService _theaterService;

        public SeatController(ISeatService seatService, ITheaterService theaterService)
        {
            _seatService = seatService;
            _theaterService = theaterService;
        }

        // GET: /Admin/Seat/Index?theaterId=5
        [HttpGet]
        public async Task<IActionResult> Index(int theaterId)
        {
            // Lấy danh sách ghế (có thể rỗng)
            var seatResponse = await _seatService.GetByTheaterAsync(theaterId);
            var seats = seatResponse.Data as List<SeatDTO> ?? new List<SeatDTO>();

            // Lấy thông tin phòng chiếu
            var theaterResponse = await _theaterService.GetByIdAsync(theaterId);

            int rows = 0;
            int columns = 0;
            string theaterName = "Không xác định";

            if (theaterResponse.Success && theaterResponse.Data is TheaterDTO theater)
            {
                rows = theater.Rows;
                columns = theater.Columns;
                theaterName = theater.Name;
            }

            var model = new
            {
                Rows = rows,
                Columns = columns,
                Seats = seats,
                TheaterName = theaterName
            };

            return PartialView("_List", model);
        }

        // GET: /Admin/Seat/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _seatService.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(response.Message);

            return PartialView("_SeatDetails", response.Data);
        }

        // POST: /Admin/Seat/Create
        [HttpPost]
        public async Task<JsonResult> Create([FromBody] SeatDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            var response = await _seatService.CreateAsync(dto);
            if (!response.Success)
                return Json(new { success = false, message = response.Message });

            return Json(new { success = true, message = response.Message, data = response.Data });
        }

        // POST: /Admin/Seat/Update/5
        [HttpPost("{id}")]
        public async Task<JsonResult> Update(int id, [FromBody] SeatDTO dto)
        {
            if (id != dto.Id)
                return Json(new { success = false, message = "ID không khớp" });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            var response = await _seatService.UpdateAsync(dto);
            if (!response.Success)
                return Json(new { success = false, message = response.Message });

            return Json(new { success = true, message = response.Message });
        }

        // POST: /Admin/Seat/Delete/5
        [HttpPost("{id}")]
        public async Task<JsonResult> Delete(int id)
        {
            var response = await _seatService.DeleteAsync(id);
            if (!response.Success)
                return Json(new { success = false, message = response.Message });

            return Json(new { success = true, message = response.Message });
        }

        // GET: /Admin/Seat/GetByTheater?theaterId=5
        [HttpGet]
        public async Task<JsonResult> GetByTheater(int theaterId)
        {
            var response = await _seatService.GetByTheaterAsync(theaterId);
            return Json(response);
        }

        // GET: /Admin/Seat/GetAll
        [HttpGet]
        public async Task<JsonResult> GetAll()
        {
            var response = await _seatService.GetAllAsync();
            return Json(response);
        }
    }
}