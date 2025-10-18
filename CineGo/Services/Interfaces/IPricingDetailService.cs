using CineGo.Models;
using CineGo.DTO.PricingDetail;

namespace CineGo.Services.Interfaces
{
    public interface IPricingDetailService
    {
        Task<ApiResponse> GetAllAsync(int page, int pageSize);
        Task<ApiResponse> GetByRuleIdAsync(int ruleId, int page, int pageSize);
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> CreateAsync(PricingDetailCreateDTO dto);
        Task<ApiResponse> UpdateAsync(PricingDetailUpdateDTO dto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}