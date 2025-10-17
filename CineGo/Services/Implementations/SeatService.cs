using CineGo.DTO.Seat;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineGo.Services.Implementations
{
    public class SeatService : ISeatService
    {
        private readonly CineGoDbContext _context;

        public SeatService(CineGoDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            var seats = await _context.Seats
                .AsNoTracking()
                .OrderBy(s => s.TheaterId)
                .ThenBy(s => s.Row)
                .ThenBy(s => s.Column)
                .Select(s => new SeatDTO
                {
                    Id = s.Id,
                    Row = s.Row,
                    Column = s.Column,
                    Type = s.Type,
                    Label = s.Label,
                    TheaterId = s.TheaterId
                })
                .ToListAsync();

            return ApiResponse.SuccessResponse(seats, "Lấy danh sách ghế thành công");
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var seat = await _context.Seats
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new SeatDTO
                {
                    Id = s.Id,
                    Row = s.Row,
                    Column = s.Column,
                    Type = s.Type,
                    Label = s.Label,
                    TheaterId = s.TheaterId
                })
                .FirstOrDefaultAsync();

            if (seat == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy ghế");

            return ApiResponse.SuccessResponse(seat, "Lấy thông tin ghế thành công");
        }

        public async Task<ApiResponse> GetByTheaterAsync(int theaterId)
        {
            var seats = await _context.Seats
                .AsNoTracking()
                .Where(s => s.TheaterId == theaterId)
                .OrderBy(s => s.Row)
                .ThenBy(s => s.Column)
                .Select(s => new SeatDTO
                {
                    Id = s.Id,
                    Row = s.Row,
                    Column = s.Column,
                    Type = s.Type,
                    Label = s.Label,
                    TheaterId = s.TheaterId
                })
                .ToListAsync();

            if (!seats.Any())
                return ApiResponse.ErrorResponse(404, "Phòng này chưa có ghế nào được cấu hình");

            return ApiResponse.SuccessResponse(seats, "Lấy danh sách ghế của phòng thành công");
        }

        public async Task<ApiResponse> CreateAsync(SeatDTO seatDto)
        {
            try
            {
                bool exists = await _context.Seats.AnyAsync(s =>
                    s.TheaterId == seatDto.TheaterId &&
                    s.Row == seatDto.Row &&
                    s.Column == seatDto.Column);

                if (exists)
                    return ApiResponse.ErrorResponse(409, "Ghế tại vị trí này đã tồn tại");

                var seat = new Models.Seat
                {
                    Row = seatDto.Row,
                    Column = seatDto.Column,
                    Type = seatDto.Type,
                    Label = seatDto.Label,
                    TheaterId = seatDto.TheaterId
                };

                _context.Seats.Add(seat);
                await _context.SaveChangesAsync();

                seatDto.Id = seat.Id;

                return ApiResponse.SuccessResponse(seatDto, "Thêm ghế mới thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse(500, $"Lỗi khi thêm ghế: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(SeatDTO seatDto)
        {
            try
            {
                var seat = await _context.Seats.FindAsync(seatDto.Id);
                if (seat == null)
                    return ApiResponse.ErrorResponse(404, "Không tìm thấy ghế để cập nhật");

                bool duplicate = await _context.Seats.AnyAsync(s =>
                    s.TheaterId == seatDto.TheaterId &&
                    s.Row == seatDto.Row &&
                    s.Column == seatDto.Column &&
                    s.Id != seatDto.Id);

                if (duplicate)
                    return ApiResponse.ErrorResponse(409, "Vị trí ghế đã được sử dụng bởi ghế khác");

                seat.Row = seatDto.Row;
                seat.Column = seatDto.Column;
                seat.Type = seatDto.Type;
                seat.Label = seatDto.Label;
                seat.TheaterId = seatDto.TheaterId;

                _context.Seats.Update(seat);
                await _context.SaveChangesAsync();

                return ApiResponse.SuccessResponse(seatDto, "Cập nhật ghế thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse(500, $"Lỗi khi cập nhật ghế: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var seat = await _context.Seats.FindAsync(id);
                if (seat == null)
                    return ApiResponse.ErrorResponse(404, "Không tìm thấy ghế để xóa");

                _context.Seats.Remove(seat);
                await _context.SaveChangesAsync();

                return ApiResponse.SuccessResponse(null, "Xóa ghế thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse(500, $"Lỗi khi xóa ghế: {ex.Message}");
            }
        }
    }
}
