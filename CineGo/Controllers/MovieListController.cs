using CineGo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CineGo.Controllers
{
    public class MovieListController : Controller
    {
        private readonly IMovieService _movieService;

        public MovieListController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Movie/NowShowing")]
        public async Task<IActionResult> NowShowing(int page = 1, int pageSize = 8)
        {
            Console.WriteLine(pageSize);
            var result = await _movieService.GetNowShowingAsync(page, pageSize);
            return Json(result);
        }

        [HttpGet("Movie/ComingSoon")]
        public async Task<IActionResult> ComingSoon(int page = 1, int pageSize = 8)
        {
            var result = await _movieService.GetComingSoonAsync(page, pageSize);
            return Json(result);
        }
    }
}
