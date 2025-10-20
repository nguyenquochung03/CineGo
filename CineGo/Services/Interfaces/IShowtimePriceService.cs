using CineGo.DTO.ShowtimePrice;
using CineGo.Models;

namespace CineGo.Services.Interfaces
{
    public interface IShowtimePriceService
    {
        Task<ApiResponse> GetByShowtimeAsync(int showtimeId, int page = 1, int pageSize = 5);
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> CreateAsync(ShowtimePriceCreateDTO dto);
        Task<ApiResponse> UpdateAsync(ShowtimePriceUpdateDTO dto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
