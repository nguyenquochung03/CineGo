using CineGo.DTO.Common;
using CineGo.DTO.PromoCode;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineGo.Services.Implementations
{
    public class PromoCodeService : IPromoCodeService
    {
        private readonly CineGoDbContext _context;

        public PromoCodeService(CineGoDbContext context)
        {
            _context = context;
        }

        // ------------------- Thêm -------------------
        public async Task<ApiResponse> AddPromoCodeAsync(PromoCodeDTO dto)
        {
            try
            {
                if (await _context.PromoCodes.AnyAsync(p => p.Code == dto.Code))
                    return ApiResponse.ErrorResponse(400, "Mã giảm giá đã tồn tại");

                var promo = new PromoCode
                {
                    Code = dto.Code,
                    DiscountType = dto.DiscountType,
                    Description = dto.Description,
                    Value = dto.Value,
                    ValidFrom = dto.ValidFrom,
                    ValidTo = dto.ValidTo,
                    UsageCount = dto.UsageCount,
                    IsActive = dto.IsActive
                };

                _context.PromoCodes.Add(promo);
                await _context.SaveChangesAsync();

                return ApiResponse.SuccessResponse(promo, "Thêm mã giảm giá thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse(500, "Lỗi khi thêm mã giảm giá", ex.Message);
            }
        }

        // ------------------- Cập nhật -------------------
        public async Task<ApiResponse> UpdatePromoCodeAsync(int id, PromoCodeDTO dto)
        {
            try
            {
                var promo = await _context.PromoCodes.FindAsync(id);
                if (promo == null)
                    return ApiResponse.ErrorResponse(404, "Không tìm thấy mã giảm giá");

                promo.Code = dto.Code;
                promo.DiscountType = dto.DiscountType;
                promo.Description = dto.Description;
                promo.Value = dto.Value;
                promo.ValidFrom = dto.ValidFrom;
                promo.ValidTo = dto.ValidTo;
                promo.UsageCount = dto.UsageCount;
                promo.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();
                return ApiResponse.SuccessResponse(promo, "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse(500, "Lỗi khi cập nhật", ex.Message);
            }
        }

        // ------------------- Xóa -------------------
        public async Task<ApiResponse> DeletePromoCodeAsync(int id)
        {
            try
            {
                var promo = await _context.PromoCodes.FindAsync(id);
                if (promo == null)
                    return ApiResponse.ErrorResponse(404, "Không tìm thấy mã giảm giá");

                _context.PromoCodes.Remove(promo);
                await _context.SaveChangesAsync();

                return ApiResponse.SuccessResponse(null, "Xóa mã giảm giá thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse(500, "Lỗi khi xóa mã giảm giá", ex.Message);
            }
        }

        // ------------------- Tìm theo mã giảm giá -------------------
        public async Task<ApiResponse> GetPromoCodeByCodeAsync(string code)
        {
            try
            {
                var promo = await _context.PromoCodes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Code == code);

                if (promo == null)
                    return ApiResponse.ErrorResponse(404, "Không tìm thấy mã giảm giá");

                var dto = new PromoCodeDTO
                {
                    Id = promo.Id,
                    Code = promo.Code,
                    DiscountType = promo.DiscountType,
                    Description = promo.Description,
                    Value = promo.Value,
                    ValidFrom = promo.ValidFrom,
                    ValidTo = promo.ValidTo,
                    UsageCount = promo.UsageCount,
                    IsActive = promo.IsActive
                };

                return ApiResponse.SuccessResponse(dto, "Tìm thấy mã giảm giá");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse(500, "Lỗi khi tìm mã giảm giá", ex.Message);
            }
        }

        // ------------------- Lọc theo loại giảm giá -------------------
        public async Task<ApiResponse> GetPromoCodesByTypeAsync(string discountType)
        {
            try
            {
                var promos = await _context.PromoCodes
                    .Where(p => p.DiscountType == discountType)
                    .Select(p => new PromoCodeDTO
                    {
                        Id = p.Id,
                        Code = p.Code,
                        DiscountType = p.DiscountType,
                        Description = p.Description,
                        Value = p.Value,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        UsageCount = p.UsageCount,
                        IsActive = p.IsActive
                    })
                    .ToListAsync();

                return ApiResponse.SuccessResponse(promos, "Danh sách theo loại giảm giá");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse(500, "Lỗi khi lọc theo loại giảm giá", ex.Message);
            }
        }

        // ------------------- Phân trang + tìm kiếm -------------------
        public async Task<ApiResponse> GetPromoCodesPagedAsync(int page, int pageSize, string? search = null)
        {
            try
            {
                var query = _context.PromoCodes.AsNoTracking();

                if (!string.IsNullOrEmpty(search))
                    query = query.Where(p => p.Code.Contains(search));

                var totalItems = await query.CountAsync();

                var promos = await query
                    .OrderByDescending(p => p.ValidTo)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PromoCodeDTO
                    {
                        Id = p.Id,
                        Code = p.Code,
                        DiscountType = p.DiscountType,
                        Description = p.Description,
                        Value = p.Value,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        UsageCount = p.UsageCount,
                        IsActive = p.IsActive
                    })
                    .ToListAsync();

                var result = new PagedResult<PromoCodeDTO>
                {
                    Items = promos,
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
                };

                return ApiResponse.SuccessResponse(result, "Lấy danh sách mã giảm giá thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse(500, "Lỗi khi lấy danh sách mã giảm giá", ex.Message);
            }
        }

        public async Task<ApiResponse> GetPromoCodeByIdAsync(int id)
        {
            try
            {
                var promo = await _context.PromoCodes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (promo == null)
                    return ApiResponse.ErrorResponse(404, "Không tìm thấy mã giảm giá");

                var dto = new PromoCodeDTO
                {
                    Id = promo.Id,
                    Code = promo.Code,
                    DiscountType = promo.DiscountType,
                    Description = promo.Description,
                    Value = promo.Value,
                    ValidFrom = promo.ValidFrom,
                    ValidTo = promo.ValidTo,
                    UsageCount = promo.UsageCount,
                    IsActive = promo.IsActive
                };

                return ApiResponse.SuccessResponse(dto, "Tìm thấy mã giảm giá");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse(500, "Lỗi khi tìm mã giảm giá", ex.Message);
            }
        }
    }
}
