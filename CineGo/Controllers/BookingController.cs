using CineGo.Services;
using CineGo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CineGo.Controllers
{
    public class BookingController : Controller
    {
        private readonly IRegionService _regionService;
        private readonly ICityService _cityService;
        private readonly ICinemaService _cinemaService;
        private readonly IMovieService _movieService;
        private readonly IShowtimeService _showtimeService;
        private readonly ISeatService _seatService;

        public BookingController(
            IRegionService regionService,
            ICityService cityService,
            ICinemaService cinemaService,
            IMovieService movieService,
            IShowtimeService showtimeService,
            ISeatService seatService)
        {
            _regionService = regionService;
            _cityService = cityService;
            _cinemaService = cinemaService;
            _movieService = movieService;
            _showtimeService = showtimeService;
            _seatService = seatService;
        }

        // GET: /Ticket
        public async Task<IActionResult> Index()
        {
            var regions = await _regionService.GetAllAsync();
            return View(regions.Data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCitiesByRegion(int regionId)
        {
            var cities = await _cityService.GetAllAsync(regionId);
            return Json(cities.Data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCinemasByCity(int cityId)
        {
            var cinemas = await _cinemaService.GetAllAsync(cityId);
            return Json(cinemas.Data);
        }

        [HttpGet]
        public async Task<JsonResult> GetMoviesByCinemaAndDate(int cinemaId, DateTime date)
        {
            var movies = await _movieService.GetByDateAndCinemaAsync(date, cinemaId);
            return Json(movies.Data);
        }

        [HttpGet]
        public async Task<JsonResult> GetShowtimesByMovieAndDate(int movieId, DateTime date)
        {
            var showtimes = await _showtimeService.GetShowtimesByDateAndMovieAsync(date, movieId);
            return Json(showtimes.Data);
        }

        [HttpGet]
        public async Task<JsonResult> GetSeatStatus(int showtimeId)
        {
            var seatData = await _seatService.CountBookedSeatsAsync(showtimeId);
            return Json(seatData.Data);
        }
    }
}
