using CineGo.DTO.TheaterTree;
using CineGo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineGo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class TheaterTreeController : Controller
    {
        private readonly ITheaterShowtimeService _service;

        public TheaterTreeController(ITheaterShowtimeService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult LoadTreeView(int showtimeId, string? showtimeTitle)
        {
            Console.WriteLine($"Loading tree view for showtimeId: {showtimeId}, showtimeTitle: {showtimeTitle}");
            ViewBag.ShowtimeId = showtimeId;
            ViewBag.ShowtimeTitle = showtimeTitle;
            return PartialView("_TheaterTree");
        }

        [HttpGet]
        public async Task<IActionResult> GetRegions(int showtimeId)
        {
            var data = await _service.GetRegionsForShowtimeAsync(showtimeId);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetCities(int regionId, int showtimeId)
        {
            var data = await _service.GetCitiesByRegionAsync(regionId, showtimeId);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetCinemas(int cityId, int showtimeId)
        {
            var data = await _service.GetCinemasByCityAsync(cityId, showtimeId);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetTheaters(int cinemaId, int showtimeId)
        {
            var data = await _service.GetTheatersByCinemaAsync(cinemaId, showtimeId);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetFullTree(int showtimeId)
        {
            var data = await _service.GetFullTreeForShowtimeAsync(showtimeId);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> GetTheaterIdsForNode([FromBody] NodeIdRequest req)
        {
            if (req == null) return BadRequest();

            var full = await _service.GetFullTreeForShowtimeAsync(req.ShowtimeId);

            var ids = new List<int>();

            if (req.NodeType == "region")
            {
                var region = full.FirstOrDefault(r => r.Id == req.NodeId);
                if (region != null)
                    ids = region.Cities.SelectMany(c => c.Cinemas).SelectMany(ci => ci.Theaters).Select(t => t.Id).ToList();
            }
            else if (req.NodeType == "city")
            {
                var city = full.SelectMany(r => r.Cities).FirstOrDefault(c => c.Id == req.NodeId);
                if (city != null)
                    ids = city.Cinemas.SelectMany(ci => ci.Theaters).Select(t => t.Id).ToList();
            }
            else if (req.NodeType == "cinema")
            {
                var cinema = full.SelectMany(r => r.Cities).SelectMany(c => c.Cinemas).FirstOrDefault(ci => ci.Id == req.NodeId);
                if (cinema != null)
                    ids = cinema.Theaters.Select(t => t.Id).ToList();
            }
            else if (req.NodeType == "theater")
            {
                ids = new List<int> { req.NodeId };
            }

            return Ok(ids);
        }

        [HttpPost]
        public async Task<IActionResult> AddTheaters([FromBody] ModifyTheatersRequest req)
        {
            if (req == null || req.TheaterIds == null || !req.TheaterIds.Any())
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            var (success, conflicts) = await _service.AddTheatersToShowtimeAsync(req.ShowtimeId, req.TheaterIds);

            if (conflicts.Any())
            {
                return Ok(new
                {
                    success = false,
                    message = "Một số phòng chiếu đang bị trùng lịch.",
                    conflicts
                });
            }

            // ✅ Không có đụng độ, nhưng cũng không có gì mới để thêm
            if (!success)
            {
                return Ok(new
                {
                    success = false,
                    message = "Không có phòng chiếu mới để thêm."
                });
            }

            // ✅ Thêm thành công
            return Ok(new
            {
                success = true,
                message = "Đã thêm phòng chiếu thành công."
            });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveTheaters([FromBody] ModifyTheatersRequest req)
        {
            if (req == null || req.TheaterIds == null || !req.TheaterIds.Any()) return BadRequest();
            var ok = await _service.RemoveTheatersFromShowtimeAsync(req.ShowtimeId, req.TheaterIds);
            if (ok) return Ok(new { success = true });
            return Ok(new { success = false, message = "Không tìm thấy theater để xóa." });
        }
    }
}
