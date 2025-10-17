using CineGo.DTO;
using CineGo.Models;

namespace CineGo.Services
{
    public interface ICityService
    {
        Task<ApiResponse> GetPagedAsync(int page = 1, int pageSize = 10, int? regionId = null);
        Task<ApiResponse> GetAllAsync(int? regionId = null);
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> CreateAsync(CityDTO dto);
        Task<ApiResponse> UpdateAsync(int id, CityDTO dto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
