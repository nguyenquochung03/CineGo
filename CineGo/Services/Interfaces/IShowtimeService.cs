using CineGo.DTO.ShowTime;
using CineGo.Models;

namespace CineGo.Services.Interfaces
{
    public interface IShowtimeService
    {
        Task<ApiResponse> GetAllAsync(int page = 1, int pageSize = 5);
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> CreateAsync(ShowtimeCreateDTO dto);
        Task<ApiResponse> UpdateAsync(ShowtimeUpdateDTO dto);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse> GetByDateAsync(DateTime date);
        Task<ApiResponse> GetByDateTimeRangeAsync(DateTime date, TimeSpan start, TimeSpan end);
        Task<ApiResponse> GetByMovieNameAsync(string movieName);
        Task<ApiResponse> GetByDateAndCinemaAsync(DateTime date, int cinemaId);
        Task<ApiResponse> GetByMovieIdAsync(int movieId);
        Task<ApiResponse> GetShowtimesByDateAndMovieAsync(DateTime date, int movieId);
    }
}
