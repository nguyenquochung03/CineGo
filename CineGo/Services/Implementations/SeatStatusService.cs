using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineGo.Services.Implementations
{
    public class SeatStatusService : ISeatStatusService
    {
        private readonly CineGoDbContext _context;

        public SeatStatusService(CineGoDbContext context)
        {
            _context = context;
        }

        // 🪑 Hàm 1: Lấy ghế đang được giữ mà chưa hết hạn
        public async Task<ApiResponse> GetActiveLockAsync(int seatId, int showtimeId)
        {
            var now = DateTime.Now;

            var seatStatus = await _context.SeatStatuses
                .Include(s => s.User)
                .Include(s => s.Seat)
                .FirstOrDefaultAsync(s =>
                    s.SeatId == seatId &&
                    s.ShowtimeId == showtimeId &&
                    (s.LockExpiresAt == null || s.LockExpiresAt > now));

            if (seatStatus == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy ghế đang được giữ hợp lệ.");

            return ApiResponse.SuccessResponse(seatStatus);
        }

        // 🆕 Hàm 2: Tạo mới SeatStatus (ghế được chọn hoặc đã thanh toán)
        public async Task<ApiResponse> CreateAsync(int seatId, int showtimeId, int userId, string status)
        {
            // Chỉ chấp nhận 2 trạng thái hợp lệ
            var allowedStatuses = new[] { "Đã chọn", "Đã thanh toán" };
            if (!allowedStatuses.Contains(status))
                return ApiResponse.ErrorResponse(400, "Trạng thái ghế không hợp lệ. Chỉ chấp nhận 'Đã chọn' hoặc 'Đã thanh toán'.");

            // Kiểm tra ghế đã tồn tại chưa
            var existing = await _context.SeatStatuses
                .FirstOrDefaultAsync(s => s.SeatId == seatId && s.ShowtimeId == showtimeId);

            if (existing != null && existing.Status == "Đã thanh toán")
                return ApiResponse.ErrorResponse(400, "Ghế này đã được thanh toán, không thể chọn lại.");

            if (existing != null && existing.LockExpiresAt > DateTime.Now)
                return ApiResponse.ErrorResponse(400, "Ghế này đang được người khác giữ.");

            // Xóa bản cũ (nếu có) trước khi thêm lại
            if (existing != null)
                _context.SeatStatuses.Remove(existing);

            var seatStatus = new SeatStatus
            {
                SeatId = seatId,
                ShowtimeId = showtimeId,
                LockedBy = userId,
                Status = status,
                LockExpiresAt = status == "Đã chọn"
                    ? DateTime.Now.AddMinutes(5)
                    : null
            };

            _context.SeatStatuses.Add(seatStatus);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(seatStatus, "Tạo trạng thái ghế thành công.");
        }

        // ❌ Hàm 3: Xóa trạng thái ghế (khi user hủy chọn hoặc hết hạn)
        public async Task<ApiResponse> DeleteAsync(int seatId, int showtimeId, int userId)
        {
            var seatStatus = await _context.SeatStatuses
                .FirstOrDefaultAsync(s =>
                    s.SeatId == seatId &&
                    s.ShowtimeId == showtimeId &&
                    s.LockedBy == userId);

            if (seatStatus == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy trạng thái ghế để xóa.");

            _context.SeatStatuses.Remove(seatStatus);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(null, "Xóa trạng thái ghế thành công.");
        }
    }
}
