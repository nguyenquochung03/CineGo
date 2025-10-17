using CineGo.DTO;
using CineGo.Models;

namespace CineGo.Services
{
    public interface IRegionService
    {
        Task<ApiResponse> GetPagedAsync(int page = 1, int pageSize = 10);
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> CreateAsync(RegionDTO dto);
        Task<ApiResponse> UpdateAsync(int id, RegionDTO dto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
