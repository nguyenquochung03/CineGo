using CineGo.DTO.Common;
using CineGo.DTO.ShowtimePrice;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CineGo.Services.Implementations
{
    public class ShowtimePriceService : IShowtimePriceService
    {
        private readonly CineGoDbContext _context;

        public ShowtimePriceService(CineGoDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetByShowtimeAsync(int showtimeId, int page = 1, int pageSize = 10)
        {
            var query = _context.ShowtimePrices.Where(p => p.ShowtimeId == showtimeId);
            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ShowtimePriceDTO
                {
                    Id = p.Id,
                    ShowtimeId = p.ShowtimeId,
                    TicketType = p.TicketType,
                    SeatType = p.SeatType,
                    Price = p.Price
                })
                .ToListAsync();

            var result = new PagedResult<ShowtimePriceDTO>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };

            return ApiResponse.SuccessResponse(result);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var price = await _context.ShowtimePrices.FindAsync(id);
            if (price == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy giá vé.");

            var dto = new ShowtimePriceDTO
            {
                Id = price.Id,
                ShowtimeId = price.ShowtimeId,
                TicketType = price.TicketType,
                SeatType = price.SeatType,
                Price = price.Price
            };

            return ApiResponse.SuccessResponse(dto);
        }

        public async Task<ApiResponse> CreateAsync(ShowtimePriceCreateDTO dto)
        {
            var showtime = await _context.Showtimes.FindAsync(dto.ShowtimeId);
            if (showtime == null)
                return ApiResponse.ErrorResponse(404, "Suất chiếu không tồn tại.");

            bool exists = await _context.ShowtimePrices
                .AnyAsync(p => p.ShowtimeId == dto.ShowtimeId
                               && p.TicketType.ToLower() == dto.TicketType.ToLower()
                               && p.SeatType.ToLower() == dto.SeatType.ToLower());
            if (exists)
                return ApiResponse.ErrorResponse(400, "Đã tồn tại giá vé với loại vé và loại ghế này trong suất chiếu.");

            var price = new ShowtimePrice
            {
                ShowtimeId = dto.ShowtimeId,
                TicketType = dto.TicketType.Trim(),
                SeatType = dto.SeatType.Trim(),
                Price = dto.Price
            };

            _context.ShowtimePrices.Add(price);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(price, "Tạo giá vé thành công.");
        }

        public async Task<ApiResponse> UpdateAsync(ShowtimePriceUpdateDTO dto)
        {
            var price = await _context.ShowtimePrices.FindAsync(dto.Id);
            if (price == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy giá vé.");

            bool exists = await _context.ShowtimePrices
                .AnyAsync(p => p.ShowtimeId == price.ShowtimeId
                               && p.Id != dto.Id
                               && p.TicketType.ToLower() == dto.TicketType.ToLower()
                               && p.SeatType.ToLower() == dto.SeatType.ToLower());
            if (exists)
                return ApiResponse.ErrorResponse(400, "Đã tồn tại giá vé với loại vé và loại ghế này trong suất chiếu.");

            price.TicketType = dto.TicketType.Trim();
            price.SeatType = dto.SeatType.Trim();
            price.Price = dto.Price;

            await _context.SaveChangesAsync();
            return ApiResponse.SuccessResponse(price, "Cập nhật giá vé thành công.");
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var price = await _context.ShowtimePrices.FindAsync(id);
            if (price == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy giá vé.");

            _context.ShowtimePrices.Remove(price);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(null, "Xóa giá vé thành công.");
        }
    }
}
