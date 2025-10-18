using CineGo.DTO.Common;
using CineGo.DTO.Helper;
using CineGo.DTO.Showtime;
using CineGo.DTO.ShowTime;
using CineGo.DTO.ShowtimePrice;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CineGo.Services.Implementations
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly CineGoDbContext _context;

        public ShowtimeService(CineGoDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetAllAsync(int page = 1, int pageSize = 5)
        {
            var query = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.PricingRule)
                .Include(s => s.TheaterShowtimes)
                    .ThenInclude(ts => ts.Theater)
                .Include(s => s.ShowtimePrices)
                .AsNoTracking();

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(s => s.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new ShowtimeDTO
                {
                    Id = s.Id,
                    MovieId = s.MovieId,
                    MovieTitle = s.Movie.Title,
                    Date = s.Date,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Format = s.Format,
                    PricingRuleId = s.PricingRuleId,
                    IsWeekend = s.IsWeekend,
                    Prices = (s.ShowtimePrices ?? new List<ShowtimePrice>())
                        .Select(p => new ShowtimePriceDTO
                        {
                            Id = p.Id,
                            ShowtimeId = s.Id,
                            TicketType = p.TicketType,
                            SeatType = p.SeatType,
                            Price = p.Price
                        })
                        .ToList(),
                    TheaterIds = s.TheaterShowtimes.Select(ts => ts.TheaterId).ToList()
                })
                .ToListAsync();

            var result = new PagedResult<ShowtimeDTO>
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
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.PricingRule)
                .Include(s => s.TheaterShowtimes)
                    .ThenInclude(ts => ts.Theater)
                .Include(s => s.ShowtimePrices)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (showtime == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy suất chiếu.");

            var dto = new ShowtimeDTO
            {
                Id = showtime.Id,
                MovieId = showtime.MovieId,
                MovieTitle = showtime.Movie.Title,
                Date = showtime.Date,
                StartTime = showtime.StartTime,
                EndTime = showtime.EndTime,
                Format = showtime.Format,
                PricingRuleId = showtime.PricingRuleId,
                IsWeekend = showtime.IsWeekend,
                Prices = showtime?.ShowtimePrices?.Select(p => new ShowtimePriceDTO
                {
                    Id = p.Id,
                    ShowtimeId = showtime.Id,
                    TicketType = p.TicketType,
                    SeatType = p.SeatType,
                    Price = p.Price
                }).ToList() ?? new List<ShowtimePriceDTO>(),
                TheaterIds = showtime?.TheaterShowtimes?.Select(ts => ts.TheaterId).ToList() ?? new List<int>()
            };

            return ApiResponse.SuccessResponse(dto);
        }

        public async Task<ApiResponse> CreateAsync(ShowtimeCreateDTO dto)
        {
            if (dto.TheaterIds == null || !dto.TheaterIds.Any())
                return ApiResponse.ErrorResponse(400, "Phải chọn ít nhất một phòng chiếu.");

            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == dto.MovieId);
            if (movie == null)
                return ApiResponse.ErrorResponse(404, "Phim không tồn tại.");

            var dayName = dto.Date.DayOfWeek.ToString();

            // Tìm pricing rule phù hợp
            var pricingRule = await _context.PricingRules
                .Include(p => p.ApplicableDays)
                .Include(p => p.PricingDetails)
                .OrderBy(p => p.Runtime)
                .FirstOrDefaultAsync(p =>
                    p.IsActive &&
                    movie.Runtime <= p.Runtime &&
                    p.ApplicableDays != null &&
                    p.ApplicableDays.Any(d => d.DayName == dayName));

            if (pricingRule == null)
                return ApiResponse.ErrorResponse(400, "Không tìm thấy quy tắc giá phù hợp. Hãy thêm PricingRule trước.");

            // Kiểm tra trùng lịch chiếu trong các phòng
            var conflictedTheaters = await GetConflictedTheatersAsync(dto.TheaterIds, dto.Date, dto.StartTime, dto.EndTime);

            if (conflictedTheaters.Any())
            {
                var message = "Các phòng chiếu trùng thời gian: " +
                              string.Join(", ", conflictedTheaters.Select(c =>
                                  $"Rạp '{c.CinemaName}' - Phòng '{c.TheaterName}' (Suất chiếu ID {c.ConflictingShowtimeId}, {c.ConflictingStart}-{c.ConflictingEnd})"));
                return ApiResponse.ErrorResponse(400, message);
            }

            bool isWeekend = dto.Date.DayOfWeek == DayOfWeek.Saturday || dto.Date.DayOfWeek == DayOfWeek.Sunday;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var showtime = new Showtime
                {
                    MovieId = dto.MovieId,
                    Date = dto.Date,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    Format = dto.Format,
                    PricingRuleId = pricingRule.Id,
                    IsWeekend = isWeekend
                };

                _context.Showtimes.Add(showtime);
                await _context.SaveChangesAsync();

                // Liên kết theater
                foreach (var theaterId in dto.TheaterIds)
                {
                    _context.TheaterShowtimes.Add(new TheaterShowtime
                    {
                        TheaterId = theaterId,
                        ShowtimeId = showtime.Id
                    });
                }

                // Tạo ShowtimePrice từ PricingDetail
                var pricingDetails = pricingRule!.PricingDetails ?? new List<PricingDetail>();

                foreach (var detail in pricingDetails)
                {
                    _context.ShowtimePrices.Add(new ShowtimePrice
                    {
                        ShowtimeId = showtime.Id,
                        TicketType = detail.TicketType,
                        SeatType = detail.SeatType,
                        Price = detail.BasePrice
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse.SuccessResponse(showtime, "Tạo suất chiếu thành công.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ApiResponse> UpdateAsync(ShowtimeUpdateDTO dto)
        {
            if (dto.TheaterIds == null || !dto.TheaterIds.Any())
                return ApiResponse.ErrorResponse(400, "Phải chọn ít nhất một phòng chiếu.");

            var showtime = await _context.Showtimes
                .Include(s => s.TheaterShowtimes)
                .Include(s => s.PricingRule)
                    .ThenInclude(p => p.PricingDetails)
                .FirstOrDefaultAsync(s => s.Id == dto.Id);

            if (showtime == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy suất chiếu.");

            var movie = await _context.Movies.FindAsync(dto.MovieId);
            if (movie == null)
                return ApiResponse.ErrorResponse(404, "Phim không tồn tại.");

            var dayName = dto.Date.DayOfWeek.ToString();

            // Tìm pricing rule phù hợp
            var pricingRule = await _context.PricingRules
                .Include(p => p.PricingDetails)
                .Include(p => p.ApplicableDays)
                .OrderBy(p => p.Runtime)
                .FirstOrDefaultAsync(p =>
                    p.IsActive &&
                    movie.Runtime <= p.Runtime &&
                    p.ApplicableDays != null &&
                    p.ApplicableDays.Any(d => d.DayName == dayName));

            if (pricingRule == null)
                return ApiResponse.ErrorResponse(400, "Không tìm thấy quy tắc giá phù hợp. Hãy thêm PricingRule trước.");

            // Kiểm tra trùng lịch chiếu trong các phòng (loại trừ showtime hiện tại)
            var conflictedTheaters = await GetConflictedTheatersAsync(dto.TheaterIds, dto.Date, dto.StartTime, dto.EndTime, showtime.Id);

            if (conflictedTheaters.Any())
            {
                var message = "Các phòng chiếu trùng thời gian: " +
                              string.Join(", ", conflictedTheaters.Select(c =>
                                  $"Rạp '{c.CinemaName}' - Phòng '{c.TheaterName}' (Suất chiếu ID {c.ConflictingShowtimeId}, {c.ConflictingStart}-{c.ConflictingEnd})"));
                return ApiResponse.ErrorResponse(400, message);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Cập nhật thông tin showtime
                showtime.MovieId = dto.MovieId;
                showtime.Date = dto.Date;
                showtime.StartTime = dto.StartTime;
                showtime.EndTime = dto.EndTime;
                showtime.Format = dto.Format;
                showtime.IsWeekend = dto.Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

                // Nếu PricingRule thay đổi
                if (showtime.PricingRuleId != pricingRule.Id)
                {
                    showtime.PricingRuleId = pricingRule.Id;

                    var oldPrices = _context.ShowtimePrices.Where(p => p.ShowtimeId == showtime.Id);
                    _context.ShowtimePrices.RemoveRange(oldPrices);

                    var pricingDetails = pricingRule!.PricingDetails ?? new List<PricingDetail>();
                    foreach (var detail in pricingDetails)
                    {
                        _context.ShowtimePrices.Add(new ShowtimePrice
                        {
                            ShowtimeId = showtime.Id,
                            TicketType = detail.TicketType,
                            SeatType = detail.SeatType,
                            Price = detail.BasePrice
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse.SuccessResponse(showtime, "Cập nhật suất chiếu thành công.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.ShowtimePrices)
                .Include(s => s.TheaterShowtimes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (showtime == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy suất chiếu.");

            _context.ShowtimePrices.RemoveRange(showtime!.ShowtimePrices!);
            _context.TheaterShowtimes.RemoveRange(showtime.TheaterShowtimes);
            _context.Showtimes.Remove(showtime);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(null, "Xóa suất chiếu thành công.");
        }

        // -------------------- Private helpers --------------------
        private async Task<List<ConflictedTheaterDTO>> GetConflictedTheatersAsync(
            List<int> theaterIds, DateTime date, TimeSpan start, TimeSpan end, int? excludeShowtimeId = null)
        {
            var query = _context.TheaterShowtimes
                .Include(ts => ts.Showtime)
                .Include(ts => ts.Theater)
                    .ThenInclude(t => t.Cinema)
                .Where(ts =>
                    theaterIds.Contains(ts.TheaterId) &&
                    ts.Showtime.Date == date &&
                    start < ts.Showtime.EndTime &&
                    end > ts.Showtime.StartTime);

            if (excludeShowtimeId.HasValue)
                query = query.Where(ts => ts.Showtime.Id != excludeShowtimeId.Value);

            return await query
                .Select(ts => new ConflictedTheaterDTO
                {
                    TheaterId = ts.TheaterId,
                    TheaterName = ts.Theater!.Name,
                    CinemaName = ts.Theater.Cinema!.Name,
                    ConflictingShowtimeId = ts.Showtime.Id,
                    ConflictingStart = ts.Showtime.StartTime,
                    ConflictingEnd = ts.Showtime.EndTime
                })
                .ToListAsync();
        }

        // -------------------- Filter methods --------------------
        public async Task<ApiResponse> GetByDateAsync(DateTime date)
        {
            var showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.PricingRule)
                .Include(s => s.TheaterShowtimes)
                    .ThenInclude(ts => ts.Theater)
                .Include(s => s.ShowtimePrices)
                .Where(s => s.Date.Date == date.Date)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            var dtos = showtimes.Select(s => MapToDTO(s)).ToList();
            return ApiResponse.SuccessResponse(dtos);
        }

        public async Task<ApiResponse> GetByDateTimeRangeAsync(DateTime date, TimeSpan start, TimeSpan end)
        {
            var showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.PricingRule)
                .Include(s => s.TheaterShowtimes)
                    .ThenInclude(ts => ts.Theater)
                .Include(s => s.ShowtimePrices)
                .Where(s => s.Date.Date == date.Date && s.StartTime >= start && s.EndTime <= end)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            var dtos = showtimes.Select(s => MapToDTO(s)).ToList();
            return ApiResponse.SuccessResponse(dtos);
        }

        public async Task<ApiResponse> GetByMovieAsync(int movieId)
        {
            var showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.PricingRule)
                .Include(s => s.TheaterShowtimes)
                    .ThenInclude(ts => ts.Theater)
                .Include(s => s.ShowtimePrices)
                .Where(s => s.MovieId == movieId)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            var dtos = showtimes.Select(s => MapToDTO(s)).ToList();
            return ApiResponse.SuccessResponse(dtos);
        }

        // -------------------- Private helper to map Showtime to DTO --------------------
        private ShowtimeDTO MapToDTO(Showtime showtime)
        {
            return new ShowtimeDTO
            {
                Id = showtime.Id,
                MovieId = showtime.MovieId,
                MovieTitle = showtime.Movie?.Title ?? "",
                Date = showtime.Date,
                StartTime = showtime.StartTime,
                EndTime = showtime.EndTime,
                Format = showtime.Format,
                PricingRuleId = showtime.PricingRuleId,
                IsWeekend = showtime.IsWeekend,
                Prices = showtime.ShowtimePrices?.Select(p => new ShowtimePriceDTO
                {
                    Id = p.Id,
                    ShowtimeId = showtime.Id,
                    TicketType = p.TicketType,
                    SeatType = p.SeatType,
                    Price = p.Price
                }).ToList() ?? new List<ShowtimePriceDTO>(),
                TheaterIds = showtime.TheaterShowtimes?.Select(ts => ts.TheaterId).ToList() ?? new List<int>()
            };
        }
    }
}
