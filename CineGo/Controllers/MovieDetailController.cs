using CineGo.DTO.Movie;
using CineGo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CineGo.Controllers
{
    public class MovieDetailController : Controller
    {
        private readonly IMovieService _movieService;

        public MovieDetailController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        // GET: /MovieDetail/Details/5
        public async Task<IActionResult> Detail(int id)
        {
            var response = await _movieService.GetByIdAsync(id);

            if (!response.Success || response.Data == null)
                return NotFound("Không tìm thấy phim.");

            var movieDTO = response.Data as MovieDTO;
            return View(movieDTO);
        }
    }
}
