using CineGo.DTO.Seat;
using CineGo.Models;

namespace CineGo.Services.Interfaces
{
    public interface ISeatService
    {
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> GetByTheaterAsync(int theaterId);
        Task<ApiResponse> CreateAsync(SeatDTO seatDto);
        Task<ApiResponse> UpdateAsync(SeatDTO seatDto);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse> CountBookedSeatsAsync(int showtimeId);
    }
}
