using CineGo.DTO.PricingRule;
using CineGo.Models;

namespace CineGo.Services.Interfaces
{
    public interface IPricingRuleService
    {
        Task<ApiResponse> GetAllAsync(int page, int pageSize);
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> CreateAsync(PricingRuleCreateDTO dto);
        Task<ApiResponse> UpdateAsync(PricingRuleUpdateDTO dto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
