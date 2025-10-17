using CineGo.DTO;
using CineGo.DTO.Common;
using CineGo.Models;
using Microsoft.EntityFrameworkCore;

namespace CineGo.Services
{
    public class TheaterService : ITheaterService
    {
        private readonly CineGoDbContext _context;

        public TheaterService(CineGoDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetPagedAsync(int page = 1, int pageSize = 5, int? cinemaId = null)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 5;

            var query = _context.Theaters.Include(t => t.Cinema).AsQueryable();

            if (cinemaId.HasValue)
                query = query.Where(t => t.CinemaId == cinemaId.Value);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var theaters = await query
                .OrderBy(t => t.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TheaterDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    Rows = t.Rows,
                    Columns = t.Columns,
                    CinemaId = t.CinemaId,
                })
                .ToListAsync();

            var result = new PagedResult<TheaterDTO>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = theaters
            };

            return ApiResponse.SuccessResponse(result);
        }

        public async Task<ApiResponse> GetAllAsync(int? cinemaId = null)
        {
            var query = _context.Theaters.Include(t => t.Cinema).AsQueryable();
            if (cinemaId.HasValue)
                query = query.Where(t => t.CinemaId == cinemaId.Value);

            var theaters = await query.Select(t => new TheaterDTO
            {
                Id = t.Id,
                Name = t.Name,
                Rows = t.Rows,
                Columns = t.Columns,
                CinemaId = t.CinemaId,
            }).ToListAsync();

            return ApiResponse.SuccessResponse(theaters);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var theater = await _context.Theaters.Include(t => t.Cinema)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (theater == null)
                return ApiResponse.ErrorResponse(404, "Theater không tồn tại.");

            var dto = new TheaterDTO
            {
                Id = theater.Id,
                Name = theater.Name,
                Rows = theater.Rows,
                Columns = theater.Columns,
                CinemaId = theater.CinemaId,
            };

            return ApiResponse.SuccessResponse(dto);
        }

        public async Task<ApiResponse> CreateAsync(TheaterDTO dto)
        {
            if (!await _context.Cinemas.AnyAsync(c => c.Id == dto.CinemaId))
                return ApiResponse.ErrorResponse(400, "Cinema không tồn tại.");

            if (await _context.Theaters.AnyAsync(t => t.Name == dto.Name && t.CinemaId == dto.CinemaId))
                return ApiResponse.ErrorResponse(400, "Tên phòng chiếu đã tồn tại trong rạp này.");

            var theater = new Theater
            {
                Name = dto.Name,
                Rows = dto.Rows,
                Columns = dto.Columns,
                CinemaId = dto.CinemaId
            };

            _context.Theaters.Add(theater);
            await _context.SaveChangesAsync();

            dto.Id = theater.Id;
            return ApiResponse.SuccessResponse(dto, "Tạo phòng chiếu thành công.");
        }

        public async Task<ApiResponse> UpdateAsync(int id, TheaterDTO dto)
        {
            var theater = await _context.Theaters.FindAsync(id);
            if (theater == null)
                return ApiResponse.ErrorResponse(404, "Theater không tồn tại.");

            if (!await _context.Cinemas.AnyAsync(c => c.Id == dto.CinemaId))
                return ApiResponse.ErrorResponse(400, "Cinema không tồn tại.");

            if (await _context.Theaters.AnyAsync(t => t.Name == dto.Name && t.CinemaId == dto.CinemaId && t.Id != id))
                return ApiResponse.ErrorResponse(400, "Tên phòng chiếu đã tồn tại trong rạp này.");

            theater.Name = dto.Name;
            theater.Rows = dto.Rows;
            theater.Columns = dto.Columns;
            theater.CinemaId = dto.CinemaId;
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(dto, "Cập nhật phòng chiếu thành công.");
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var theater = await _context.Theaters.Include(t => t.Seats).Include(t => t.Showtimes)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (theater == null)
                return ApiResponse.ErrorResponse(404, "Theater không tồn tại.");

            if ((theater.Seats != null && theater.Seats.Any()) ||
                (theater.Showtimes != null && theater.Showtimes.Any()))
            {
                return ApiResponse.ErrorResponse(400, "Không thể xóa phòng chiếu có ghế hoặc suất chiếu.");
            }

            _context.Theaters.Remove(theater);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(null, "Xóa phòng chiếu thành công.");
        }
    }
}
