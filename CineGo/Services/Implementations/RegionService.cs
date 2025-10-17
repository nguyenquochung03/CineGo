using CineGo.DTO;
using CineGo.DTO.Common;
using CineGo.Models;
using Microsoft.EntityFrameworkCore;

namespace CineGo.Services.Implementations
{
    public class RegionService : IRegionService
    {
        private readonly CineGoDbContext _context;

        public RegionService(CineGoDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetPagedAsync(int page = 1, int pageSize = 5)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 5;

            var totalItems = await _context.Regions.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var regions = await _context.Regions
                .OrderBy(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RegionDTO
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToListAsync();

            var result = new PagedResult<RegionDTO>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = regions
            };

            return ApiResponse.SuccessResponse(result);
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            var regions = await _context.Regions
                .Include(r => r.Cities)
                .Select(r => new RegionDTO
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToListAsync();
            return ApiResponse.SuccessResponse(regions);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var region = await _context.Regions
                .Include(r => r.Cities)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (region == null)
                return ApiResponse.ErrorResponse(404, "Region không tồn tại.");

            var dto = new RegionDTO
            {
                Id = region.Id,
                Name = region.Name
            };

            return ApiResponse.SuccessResponse(dto);
        }

        public async Task<ApiResponse> CreateAsync(RegionDTO dto)
        {
            if (await _context.Regions.AnyAsync(r => r.Name == dto.Name))
                return ApiResponse.ErrorResponse(400, "Tên vùng đã tồn tại.");

            var region = new Region
            {
                Name = dto.Name
            };

            _context.Regions.Add(region);
            await _context.SaveChangesAsync();

            dto.Id = region.Id;
            return ApiResponse.SuccessResponse(dto, "Tạo vùng thành công.");
        }

        public async Task<ApiResponse> UpdateAsync(int id, RegionDTO dto)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region == null)
                return ApiResponse.ErrorResponse(404, "Region không tồn tại.");

            if (await _context.Regions.AnyAsync(r => r.Name == dto.Name && r.Id != id))
                return ApiResponse.ErrorResponse(400, "Tên vùng đã tồn tại.");

            region.Name = dto.Name;
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(dto, "Cập nhật vùng thành công.");
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var region = await _context.Regions
                .Include(r => r.Cities)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (region == null)
                return ApiResponse.ErrorResponse(404, "Region không tồn tại.");

            if (region.Cities != null && region.Cities.Any())
                return ApiResponse.ErrorResponse(400, "Không thể xóa vùng có thành phố.");

            _context.Regions.Remove(region);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(null, "Xóa vùng thành công.");
        }
    }
}
