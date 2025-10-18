using CineGo.DTO.Common;
using CineGo.DTO.PricingDetail;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace CineGo.Services.Implementations
{
    public class PricingDetailService : IPricingDetailService
    {
        private readonly CineGoDbContext _context;
        public PricingDetailService(CineGoDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetAllAsync(int page, int pageSize)
        {
            var query = _context.PricingDetails
                .Include(x => x.PricingRule)
                .AsNoTracking()
                .OrderBy(x => x.TicketType);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new PricingDetailDTO
                {
                    Id = x.Id,
                    TicketType = x.TicketType,
                    SeatType = x.SeatType,
                    BasePrice = x.BasePrice,
                    PricingRuleId = x.PricingRuleId,
                }).ToListAsync();

            var result = new PagedResult<PricingDetailDTO>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return ApiResponse.SuccessResponse(result);
        }

        public async Task<ApiResponse> GetByRuleIdAsync(int ruleId, int page, int pageSize)
        {
            var query = _context.PricingDetails
                .Include(x => x.PricingRule)
                .AsNoTracking()
                .Where(x => x.PricingRuleId == ruleId)
                .OrderBy(x => x.TicketType);

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new PricingDetailDTO
                {
                    Id = x.Id,
                    TicketType = x.TicketType,
                    SeatType = x.SeatType,
                    BasePrice = x.BasePrice,
                    PricingRuleId = x.PricingRuleId,
                })
                .ToListAsync();

            var result = new PagedResult<PricingDetailDTO>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return ApiResponse.SuccessResponse(result);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var detail = await _context.PricingDetails
                .Include(x => x.PricingRule)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (detail == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy chi tiết giá.");

            var dto = new PricingDetailDTO
            {
                Id = detail.Id,
                TicketType = detail.TicketType,
                SeatType = detail.SeatType,
                BasePrice = detail.BasePrice,
                PricingRuleId = detail.PricingRuleId,
            };

            return ApiResponse.SuccessResponse(dto);
        }

        public async Task<ApiResponse> CreateAsync(PricingDetailCreateDTO dto)
        {
            bool exists = await _context.PricingDetails.AnyAsync(x =>
                x.PricingRuleId == dto.PricingRuleId &&
                x.TicketType == dto.TicketType &&
                x.SeatType == dto.SeatType);

            if (exists)
                return ApiResponse.ErrorResponse(400, "Chi tiết giá đã tồn tại cho loại vé và ghế này.");

            var detail = new PricingDetail
            {
                TicketType = dto.TicketType,
                SeatType = dto.SeatType,
                BasePrice = dto.BasePrice,
                PricingRuleId = dto.PricingRuleId
            };

            _context.PricingDetails.Add(detail);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(detail.Id, "Tạo chi tiết giá thành công.");
        }

        public async Task<ApiResponse> UpdateAsync(PricingDetailUpdateDTO dto)
        {
            var detail = await _context.PricingDetails.FindAsync(dto.Id);
            if (detail == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy chi tiết giá.");

            detail.TicketType = dto.TicketType;
            detail.SeatType = dto.SeatType;
            detail.BasePrice = dto.BasePrice;
            detail.PricingRuleId = dto.PricingRuleId;

            await _context.SaveChangesAsync();
            return ApiResponse.SuccessResponse(null, "Cập nhật chi tiết giá thành công.");
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var detail = await _context.PricingDetails.FindAsync(id);
            if (detail == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy chi tiết giá.");

            _context.PricingDetails.Remove(detail);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(null, "Xóa chi tiết giá thành công.");
        }
    }
}
