using CineGo.DTO;
using CineGo.DTO.Common;
using CineGo.Models;
using Microsoft.EntityFrameworkCore;

namespace CineGo.Services.Implementations
{
    public class CityService : ICityService
    {
        private readonly CineGoDbContext _context;

        public CityService(CineGoDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetPagedAsync(int page = 1, int pageSize = 5, int? regionId = null)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 5;

            var query = _context.Cities.Include(c => c.Region).AsQueryable();

            if (regionId.HasValue)
                query = query.Where(c => c.RegionId == regionId.Value);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var cities = await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CityDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    RegionId = c.RegionId,
                })
                .ToListAsync();

            var result = new PagedResult<CityDTO>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = cities
            };

            return ApiResponse.SuccessResponse(result);
        }

        public async Task<ApiResponse> GetAllAsync(int? regionId = null)
        {
            var query = _context.Cities.Include(c => c.Region).AsQueryable();
            if (regionId.HasValue)
                query = query.Where(c => c.RegionId == regionId.Value);

            var cities = await query.Select(c => new CityDTO
            {
                Id = c.Id,
                Name = c.Name,
                RegionId = c.RegionId,
            }).ToListAsync();

            return ApiResponse.SuccessResponse(cities);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var city = await _context.Cities.Include(c => c.Region)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (city == null)
                return ApiResponse.ErrorResponse(404, "City không tồn tại.");

            var dto = new CityDTO
            {
                Id = city.Id,
                Name = city.Name,
                RegionId = city.RegionId,
            };

            return ApiResponse.SuccessResponse(dto);
        }

        public async Task<ApiResponse> CreateAsync(CityDTO dto)
        {
            if (!await _context.Regions.AnyAsync(r => r.Id == dto.RegionId))
                return ApiResponse.ErrorResponse(400, "Region không tồn tại.");

            if (await _context.Cities.AnyAsync(c => c.Name == dto.Name && c.RegionId == dto.RegionId))
                return ApiResponse.ErrorResponse(400, "Tên thành phố đã tồn tại trong vùng này.");

            var city = new City
            {
                Name = dto.Name,
                RegionId = dto.RegionId
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            dto.Id = city.Id;
            return ApiResponse.SuccessResponse(dto, "Tạo thành phố thành công.");
        }

        public async Task<ApiResponse> UpdateAsync(int id, CityDTO dto)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
                return ApiResponse.ErrorResponse(404, "City không tồn tại.");

            if (!await _context.Regions.AnyAsync(r => r.Id == dto.RegionId))
                return ApiResponse.ErrorResponse(400, "Region không tồn tại.");

            if (await _context.Cities.AnyAsync(c => c.Name == dto.Name && c.RegionId == dto.RegionId && c.Id != id))
                return ApiResponse.ErrorResponse(400, "Tên thành phố đã tồn tại trong vùng này.");

            city.Name = dto.Name;
            city.RegionId = dto.RegionId;
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(dto, "Cập nhật thành phố thành công.");
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var city = await _context.Cities.Include(c => c.Cinemas).FirstOrDefaultAsync(c => c.Id == id);
            if (city == null)
                return ApiResponse.ErrorResponse(404, "City không tồn tại.");

            if (city.Cinemas != null && city.Cinemas.Any())
                return ApiResponse.ErrorResponse(400, "Không thể xóa thành phố có rạp chiếu.");

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(null, "Xóa thành phố thành công.");
        }
    }
}
