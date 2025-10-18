using CineGo.DTO.PricingRuleDay;
using CineGo.Models;

namespace CineGo.Services.Interfaces
{
    public interface IPricingRuleDayService
    {
        Task<ApiResponse> GetAllAsync(int page, int pageSize);
        Task<ApiResponse> GetByRuleIdAsync(int ruleId, int page, int pageSize);
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> CreateAsync(PricingRuleDayCreateDTO dto);
        Task<ApiResponse> UpdateAsync(PricingRuleDayUpdateDTO dto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
