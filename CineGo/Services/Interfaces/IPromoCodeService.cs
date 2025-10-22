using CineGo.DTO.PromoCode;
using CineGo.Models;

namespace CineGo.Services.Interfaces
{
    public interface IPromoCodeService
    {
        Task<ApiResponse> AddPromoCodeAsync(PromoCodeDTO dto);
        Task<ApiResponse> UpdatePromoCodeAsync(int id, PromoCodeDTO dto);
        Task<ApiResponse> DeletePromoCodeAsync(int id);
        Task<ApiResponse> GetPromoCodeByCodeAsync(string code);
        Task<ApiResponse> GetPromoCodesPagedAsync(int page, int pageSize, string? search = null);
        Task<ApiResponse> GetPromoCodesByTypeAsync(string discountType);
        Task<ApiResponse> GetPromoCodeByIdAsync(int id);
    }
}
