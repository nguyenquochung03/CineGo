using CineGo.DTO.Common;
using CineGo.DTO.PricingRule;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace CineGo.Services.Implementations
{
    public class PricingRuleService : IPricingRuleService
    {
        private readonly CineGoDbContext _context;
        public PricingRuleService(CineGoDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetAllAsync(int page, int pageSize)
        {
            var query = _context.PricingRules.AsNoTracking().OrderByDescending(x => x.CreatedAt);
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new PricingRuleDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Runtime = x.Runtime,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                }).ToListAsync();

            var result = new PagedResult<PricingRuleDTO>
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
            var rule = await _context.PricingRules.FindAsync(id);
            if (rule == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy quy tắc giá.");

            var dto = new PricingRuleDTO
            {
                Id = rule.Id,
                Name = rule.Name,
                Description = rule.Description,
                Runtime = rule.Runtime,
                IsActive = rule.IsActive,
                CreatedAt = rule.CreatedAt
            };

            return ApiResponse.SuccessResponse(dto);
        }

        public async Task<ApiResponse> CreateAsync(PricingRuleCreateDTO dto)
        {
            if (await _context.PricingRules.AnyAsync(x => x.Name == dto.Name))
                return ApiResponse.ErrorResponse(400, "Tên quy tắc giá đã tồn tại.");

            var rule = new PricingRule
            {
                Name = dto.Name,
                Description = dto.Description,
                Runtime = dto.Runtime,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.PricingRules.Add(rule);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(rule.Id, "Tạo quy tắc giá thành công.");
        }

        public async Task<ApiResponse> UpdateAsync(PricingRuleUpdateDTO dto)
        {
            var rule = await _context.PricingRules.FindAsync(dto.Id);
            if (rule == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy quy tắc giá.");

            rule.Name = dto.Name;
            rule.Description = dto.Description;
            rule.Runtime = dto.Runtime;
            rule.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return ApiResponse.SuccessResponse(null, "Cập nhật thành công.");
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var rule = await _context.PricingRules.FindAsync(id);
            if (rule == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy quy tắc giá.");

            _context.PricingRules.Remove(rule);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(null, "Xóa quy tắc giá thành công.");
        }
    }
}
