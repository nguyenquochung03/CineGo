using CineGo.DTO.Common;
using CineGo.DTO.PricingRuleDay;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace CineGo.Services.Implementations
{
    public class PricingRuleDayService : IPricingRuleDayService
    {
        private readonly CineGoDbContext _context;
        public PricingRuleDayService(CineGoDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetAllAsync(int page, int pageSize)
        {
            var query = _context.PricingRuleDays.Include(x => x.PricingRule).AsNoTracking();
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new PricingRuleDayDTO
                {
                    Id = x.Id,
                    DayName = x.DayName,
                    PricingRuleId = x.PricingRuleId,
                }).ToListAsync();

            var result = new PagedResult<PricingRuleDayDTO>
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
            var query = _context.PricingRuleDays
                .Include(x => x.PricingRule)
                .AsNoTracking()
                .Where(x => x.PricingRuleId == ruleId)
                .OrderBy(x => x.DayName);

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new PricingRuleDayDTO
                {
                    Id = x.Id,
                    DayName = x.DayName,
                    PricingRuleId = x.PricingRuleId,
                })
                .ToListAsync();

            var result = new PagedResult<PricingRuleDayDTO>
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
            var day = await _context.PricingRuleDays
                .Include(x => x.PricingRule)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (day == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy ngày áp dụng.");

            var dto = new PricingRuleDayDTO
            {
                Id = day.Id,
                DayName = day.DayName,
                PricingRuleId = day.PricingRuleId,
            };

            return ApiResponse.SuccessResponse(dto);
        }

        public async Task<ApiResponse> CreateAsync(PricingRuleDayCreateDTO dto)
        {
            bool exists = await _context.PricingRuleDays.AnyAsync(x =>
                x.PricingRuleId == dto.PricingRuleId && x.DayName == dto.DayName);

            if (exists)
                return ApiResponse.ErrorResponse(400, "Ngày áp dụng này đã tồn tại cho quy tắc giá.");

            var day = new PricingRuleDay
            {
                DayName = dto.DayName,
                PricingRuleId = dto.PricingRuleId
            };

            _context.PricingRuleDays.Add(day);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(day.Id, "Thêm ngày áp dụng thành công.");
        }

        public async Task<ApiResponse> UpdateAsync(PricingRuleDayUpdateDTO dto)
        {
            var day = await _context.PricingRuleDays.FindAsync(dto.Id);
            if (day == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy ngày áp dụng.");

            day.DayName = dto.DayName;
            day.PricingRuleId = dto.PricingRuleId;

            await _context.SaveChangesAsync();
            return ApiResponse.SuccessResponse(null, "Cập nhật thành công.");
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var day = await _context.PricingRuleDays.FindAsync(id);
            if (day == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy ngày áp dụng.");

            _context.PricingRuleDays.Remove(day);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(null, "Xóa ngày áp dụng thành công.");
        }
    }
}
