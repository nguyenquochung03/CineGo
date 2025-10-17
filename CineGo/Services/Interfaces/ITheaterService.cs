using CineGo.DTO;
using CineGo.Models;

namespace CineGo.Services
{
    public interface ITheaterService
    {
        Task<ApiResponse> GetPagedAsync(int page = 1, int pageSize = 10, int? cinemaId = null);
        Task<ApiResponse> GetAllAsync(int? cinemaId = null);
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> CreateAsync(TheaterDTO dto);
        Task<ApiResponse> UpdateAsync(int id, TheaterDTO dto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
