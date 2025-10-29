using CineGo.Models;

namespace CineGo.Services.Interfaces
{
    public interface ISeatStatusService
    {
        Task<ApiResponse> GetActiveLockAsync(int seatId, int showtimeId);
        Task<ApiResponse> CreateAsync(int seatId, int showtimeId, int userId, string status);
        Task<ApiResponse> DeleteAsync(int seatId, int showtimeId, int userId);
    }
}
