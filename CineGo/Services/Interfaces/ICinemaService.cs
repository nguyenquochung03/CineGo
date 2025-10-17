using CineGo.DTO;
using CineGo.Models;

namespace CineGo.Services
{
    public interface ICinemaService
    {
        Task<ApiResponse> GetPagedAsync(int page = 1, int pageSize = 10, int? cityId = null);
        Task<ApiResponse> GetAllAsync(int? cityId = null);
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> CreateAsync(CinemaDTO dto);
        Task<ApiResponse> UpdateAsync(int id, CinemaDTO dto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
