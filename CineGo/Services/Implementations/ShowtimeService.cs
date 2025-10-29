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
            };

            return ApiResponse.SuccessResponse(dto);
        }

        public async Task<ApiResponse> CreateAsync(ShowtimeCreateDTO dto)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == dto.MovieId);
            if (movie == null)
                return ApiResponse.ErrorResponse(404, "Phim không tồn tại.");

            // Check ngày trong quá khứ
            if (dto.Date.Date < DateTime.Today)
                return ApiResponse.ErrorResponse(400, "Ngày chiếu không thể là quá khứ.");

            if (dto.StartTime >= dto.EndTime)
                return ApiResponse.ErrorResponse(400, "Giờ bắt đầu phải nhỏ hơn giờ kết thúc.");

            var dayName = dto.Date.DayOfWeek.ToString();

            // Tìm pricing rule phù hợp
            var pricingRule = await _context.PricingRules
                .Include(p => p.ApplicableDays)
                .Include(p => p.PricingDetails)
                .OrderBy(p => p.Runtime)
                .FirstOrDefaultAsync(p =>
                    p.IsActive &&
                    movie.Runtime <= p.Runtime &&
                    p.ApplicableDays.Any(d => d.DayName == dayName));

            if (pricingRule == null)
            {
                // Kiểm tra từng loại nguyên nhân
                bool hasRuntimeRule = await _context.PricingRules.AnyAsync(p => p.IsActive && movie.Runtime <= p.Runtime);
                bool hasDayRule = await _context.PricingRules
                    .Include(p => p.ApplicableDays)
                    .AnyAsync(p => p.IsActive && p.ApplicableDays.Any(d => d.DayName == dayName));

                // Xây dựng message chi tiết
                var reasons = new List<string>();
                if (!hasRuntimeRule)
                    reasons.Add($"Không có đơn giá phù hợp (phim dài {movie.Runtime} phút).");
                if (!hasDayRule)
                    reasons.Add($"Không có đơn giá áp dụng cho ngày {dayName}.");

                string reason = string.Join(Environment.NewLine, reasons);
                return ApiResponse.ErrorResponse(400, reason);
            }

            // Kiểm tra trùng suất chiếu
            bool isDuplicate = await _context.Showtimes
                .AnyAsync(s =>
                    s.MovieId == dto.MovieId &&
                    s.Date == dto.Date &&
                    s.StartTime == dto.StartTime &&
                    s.Format == dto.Format
                );

            if (isDuplicate)
                return ApiResponse.ErrorResponse(400, $"Suất chiếu cho phim này ({dto.Format}) vào giờ đã tồn tại trong cùng ngày.");

            bool isWeekend = dto.Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

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

                // Tạo giá vé mặc định dựa trên PricingRule
                var pricingDetails = pricingRule.PricingDetails ?? new List<PricingDetail>();
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

                var responseDto = new ShowtimeDTO
                {
                    Id = showtime.Id,
                    MovieId = showtime.MovieId,
                    MovieTitle = movie.Title,
                    Date = showtime.Date,
                    StartTime = showtime.StartTime,
                    EndTime = showtime.EndTime,
                    Format = showtime.Format,
                    IsWeekend = showtime.IsWeekend
                };

                return ApiResponse.SuccessResponse(responseDto, "Tạo suất chiếu thành công (chưa gán phòng chiếu).");

            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ApiResponse> UpdateAsync(ShowtimeUpdateDTO dto)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.PricingRule)
                    .ThenInclude(p => p.PricingDetails)
                .FirstOrDefaultAsync(s => s.Id == dto.Id);

            if (showtime == null)
                return ApiResponse.ErrorResponse(404, "Không tìm thấy suất chiếu.");

            var movie = await _context.Movies.FindAsync(dto.MovieId);
            if (movie == null)
                return ApiResponse.ErrorResponse(404, "Phim không tồn tại.");

            if (dto.Date.Date < DateTime.Today)
                return ApiResponse.ErrorResponse(400, "Ngày chiếu không thể là quá khứ.");

            if (dto.StartTime >= dto.EndTime)
                return ApiResponse.ErrorResponse(400, "Giờ bắt đầu phải nhỏ hơn giờ kết thúc.");

            var dayName = dto.Date.DayOfWeek.ToString();

            var pricingRule = await _context.PricingRules
                .Include(p => p.PricingDetails)
                .Include(p => p.ApplicableDays)
                .OrderBy(p => p.Runtime)
                .FirstOrDefaultAsync(p =>
                    p.IsActive &&
                    movie.Runtime <= p.Runtime &&
                    p.ApplicableDays.Any(d => d.DayName == dayName));

            if (pricingRule == null)
            {
                // Kiểm tra từng loại nguyên nhân
                bool hasRuntimeRule = await _context.PricingRules.AnyAsync(p => p.IsActive && movie.Runtime <= p.Runtime);
                bool hasDayRule = await _context.PricingRules
                    .Include(p => p.ApplicableDays)
                    .AnyAsync(p => p.IsActive && p.ApplicableDays.Any(d => d.DayName == dayName));

                // Xây dựng message chi tiết
                var reasons = new List<string>();
                if (!hasRuntimeRule)
                    reasons.Add($"Không có đơn giá phù hợp (phim dài {movie.Runtime} phút).");
                if (!hasDayRule)
                    reasons.Add($"Không có đơn giá áp dụng cho ngày {dayName}.");

                string reason = string.Join(Environment.NewLine, reasons);
                return ApiResponse.ErrorResponse(400, reason);
            }

            // Kiểm tra trùng suất chiếu
            bool isDuplicate = await _context.Showtimes
                .AnyAsync(s =>
                    s.Id != dto.Id &&
                    s.MovieId == dto.MovieId &&
                    s.Date == dto.Date &&
                    s.StartTime == dto.StartTime &&
                    s.Format == dto.Format
                );

            if (isDuplicate)
                return ApiResponse.ErrorResponse(400, $"Suất chiếu cho phim này ({dto.Format}) vào giờ đã tồn tại trong cùng ngày.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
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

                    var pricingDetails = pricingRule.PricingDetails ?? new List<PricingDetail>();
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

                var responseDto = new ShowtimeDTO
                {
                    Id = showtime.Id,
                    MovieId = showtime.MovieId,
                    MovieTitle = movie.Title,
                    Date = showtime.Date,
                    StartTime = showtime.StartTime,
                    EndTime = showtime.EndTime,
                    Format = showtime.Format,
                    IsWeekend = showtime.IsWeekend
                };

                return ApiResponse.SuccessResponse(responseDto, "Tạo suất chiếu thành công (chưa gán phòng chiếu).");
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

            _context.ShowtimePrices.RemoveRange(showtime.ShowtimePrices);
            _context.TheaterShowtimes.RemoveRange(showtime.TheaterShowtimes);
            _context.Showtimes.Remove(showtime);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(null, "Xóa suất chiếu thành công.");
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
                .Where(s => s.Date.Date == date.Date && s.EndTime > start && s.StartTime < end)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            var dtos = showtimes.Select(s => MapToDTO(s)).ToList();
            return ApiResponse.SuccessResponse(dtos);
        }

        public async Task<ApiResponse> GetByMovieNameAsync(string movieName)
        {
            if (string.IsNullOrWhiteSpace(movieName))
                return ApiResponse.ErrorResponse(400, "Tên phim không được để trống.");

            var showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.PricingRule)
                .Include(s => s.TheaterShowtimes)
                    .ThenInclude(ts => ts.Theater)
                .Include(s => s.ShowtimePrices)
                .AsNoTracking()
                .Where(s => s.Movie.Title.ToLower().Contains(movieName.ToLower()))
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            if (!showtimes.Any())
                return ApiResponse.ErrorResponse(404, $"Không tìm thấy suất chiếu cho phim có tên chứa '{movieName}'.");

            var dtos = showtimes.Select(s => MapToDTO(s)).ToList();
            return ApiResponse.SuccessResponse(dtos);
        }

        public async Task<ApiResponse> GetByDateAndCinemaAsync(DateTime date, int cinemaId)
        {
            var showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.PricingRule)
                .Include(s => s.TheaterShowtimes)
                    .ThenInclude(ts => ts.Theater)
                .Where(s => s.Date.Date == date.Date &&
                            s.TheaterShowtimes.Any(ts => ts.Theater.CinemaId == cinemaId))
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            if (!showtimes.Any())
                return ApiResponse.ErrorResponse(404, "Không tìm thấy suất chiếu cho rạp này trong ngày đã chọn.");

            var dtos = showtimes.Select(s => MapToDTO(s)).ToList();

            return ApiResponse.SuccessResponse(dtos);
        }

        public async Task<ApiResponse> GetByMovieIdAsync(int movieId)
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

            if (!showtimes.Any())
                return ApiResponse.ErrorResponse(404, $"Không tìm thấy suất chiếu cho phim có Id = {movieId}.");

            var dtos = showtimes.Select(s => new ShowtimeDTO
            {
                Id = s.Id,
                MovieId = s.MovieId,
                MovieTitle = s.Movie?.Title ?? "",
                Date = s.Date,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Format = s.Format,
                PricingRuleId = s.PricingRuleId,
                IsWeekend = s.IsWeekend,
            }).ToList();

            return ApiResponse.SuccessResponse(dtos);
        }

        public async Task<ApiResponse> GetShowtimesByDateAndMovieAsync(DateTime date, int movieId)
        {
            try
            {
                var targetDate = date.Date;

                var showtimes = await _context.Showtimes
                    .Include(s => s.Movie)
                    .Include(s => s.TheaterShowtimes)
                        .ThenInclude(ts => ts.Theater)
                            .ThenInclude(t => t.Cinema)
                    .Where(s =>
                        s.MovieId == movieId &&
                        s.Date.Date == targetDate)
                    .OrderBy(s => s.StartTime)
                    .Select(s => new
                    {
                        s.Id,
                        s.MovieId,
                        MovieTitle = s.Movie.Title,
                        Theaters = s.TheaterShowtimes.Select(ts => new
                        {
                            ts.Theater.Id,
                            TheaterName = ts.Theater.Name,
                            CinemaName = ts.Theater.Cinema.Name
                        }),
                        s.Date,
                        s.StartTime,
                        s.EndTime,
                        s.Format
                    })
                    .ToListAsync();

                if (showtimes == null || !showtimes.Any())
                    return ApiResponse.ErrorResponse(404, "Không tìm thấy suất chiếu nào cho phim và ngày đã chọn.");

                return ApiResponse.SuccessResponse(showtimes);
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse(500, $"Lỗi khi lấy suất chiếu: {ex.Message}");
            }
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
            };
        }
    }
}
