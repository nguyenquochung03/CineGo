using CineGo.DTO;
using CineGo.DTO.Common;
using CineGo.Models;
using Microsoft.EntityFrameworkCore;

namespace CineGo.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly CineGoDbContext _context;

        public CinemaService(CineGoDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetPagedAsync(int page = 1, int pageSize = 5, int? cityId = null)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 5;

            var query = _context.Cinemas.Include(c => c.City).AsQueryable();

            if (cityId.HasValue)
                query = query.Where(c => c.CityId == cityId.Value);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var cinemas = await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CinemaDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Address = c.Address,
                    CityId = c.CityId,
                })
                .ToListAsync();

            var result = new PagedResult<CinemaDTO>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = cinemas
            };

            return ApiResponse.SuccessResponse(result);
        }

        public async Task<ApiResponse> GetAllAsync(int? cityId = null)
        {
            var query = _context.Cinemas.Include(c => c.City).AsQueryable();
            if (cityId.HasValue)
                query = query.Where(c => c.CityId == cityId.Value);

            var cinemas = await query.Select(c => new CinemaDTO
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                CityId = c.CityId,
            }).ToListAsync();

            return ApiResponse.SuccessResponse(cinemas);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var cinema = await _context.Cinemas.Include(c => c.City)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cinema == null)
                return ApiResponse.ErrorResponse(404, "Cinema không tồn tại.");

            var dto = new CinemaDTO
            {
                Id = cinema.Id,
                Name = cinema.Name,
                Address = cinema.Address,
                CityId = cinema.CityId,
            };

            return ApiResponse.SuccessResponse(dto);
        }

        public async Task<ApiResponse> CreateAsync(CinemaDTO dto)
        {
            if (!await _context.Cities.AnyAsync(c => c.Id == dto.CityId))
                return ApiResponse.ErrorResponse(400, "City không tồn tại.");

            if (await _context.Cinemas.AnyAsync(c => c.Name == dto.Name && c.CityId == dto.CityId))
                return ApiResponse.ErrorResponse(400, "Tên rạp chiếu đã tồn tại trong thành phố này.");

            var cinema = new Cinema
            {
                Name = dto.Name,
                Address = dto.Address,
                CityId = dto.CityId
            };

            _context.Cinemas.Add(cinema);
            await _context.SaveChangesAsync();

            dto.Id = cinema.Id;
            return ApiResponse.SuccessResponse(dto, "Tạo rạp chiếu thành công.");
        }

        public async Task<ApiResponse> UpdateAsync(int id, CinemaDTO dto)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null)
                return ApiResponse.ErrorResponse(404, "Cinema không tồn tại.");

            if (!await _context.Cities.AnyAsync(c => c.Id == dto.CityId))
                return ApiResponse.ErrorResponse(400, "City không tồn tại.");

            if (await _context.Cinemas.AnyAsync(c => c.Name == dto.Name && c.CityId == dto.CityId && c.Id != id))
                return ApiResponse.ErrorResponse(400, "Tên rạp chiếu đã tồn tại trong thành phố này.");

            cinema.Name = dto.Name;
            cinema.Address = dto.Address;
            cinema.CityId = dto.CityId;
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(dto, "Cập nhật rạp chiếu thành công.");
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var cinema = await _context.Cinemas.Include(c => c.Theaters).FirstOrDefaultAsync(c => c.Id == id);
            if (cinema == null)
                return ApiResponse.ErrorResponse(404, "Cinema không tồn tại.");

            if (cinema.Theaters != null && cinema.Theaters.Any())
                return ApiResponse.ErrorResponse(400, "Không thể xóa rạp chiếu có phòng chiếu.");

            _context.Cinemas.Remove(cinema);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(null, "Xóa rạp chiếu thành công.");
        }
    }
}
