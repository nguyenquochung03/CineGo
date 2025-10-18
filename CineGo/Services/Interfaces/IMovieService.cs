using CineGo.DTO.Movie;
using CineGo.Models;

namespace CineGo.Services.Interfaces
{
    public interface IMovieService
    {
        Task<ApiResponse> GetPagedAsync(int page = 1, int pageSize = 10);
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> CreateAsync(MovieCreateUpdateDTO movie);
        Task<ApiResponse> UpdateAsync(int id, MovieCreateUpdateDTO movie);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse> SearchByTitleAsync(string title, int page = 1, int pageSize = 10);
    }
}
