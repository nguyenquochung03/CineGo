using CineGo.DTO.Helper;
using CineGo.DTO.TheaterTree;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineGo.Services.Implementations
{
    public class TheaterShowtimeService : ITheaterShowtimeService
    {
        private readonly CineGoDbContext _context;

        public TheaterShowtimeService(CineGoDbContext context)
        {
            _context = context;
        }

        // Cấp 1: Region
        public async Task<List<RegionNodeDto>> GetRegionsForShowtimeAsync(int showtimeId)
        {
            var selectedIds = await _context.TheaterShowtimes
                .Where(ts => ts.ShowtimeId == showtimeId)
                .Select(ts => ts.TheaterId)
                .ToListAsync();

            var regions = await _context.Regions
                .Include(r => r.Cities!)
                    .ThenInclude(c => c.Cinemas!)
                        .ThenInclude(ci => ci.Theaters!)
                .ToListAsync();

            return regions
                .Where(r => r.Cities!.Any(c => c.Cinemas!.Any(ci => ci.Theaters!.Any())))
                .Select(r => new RegionNodeDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    TotalCount = r.Cities!.SelectMany(c => c.Cinemas!).SelectMany(ci => ci.Theaters!).Count(),
                    SelectedCount = r.Cities!.SelectMany(c => c.Cinemas!).SelectMany(ci => ci.Theaters!).Count(t => selectedIds.Contains(t.Id))
                })
                .ToList();
        }

        // Cấp 2: City
        public async Task<List<CityNodeDto>> GetCitiesByRegionAsync(int regionId, int showtimeId)
        {
            var selectedIds = await _context.TheaterShowtimes
                .Where(ts => ts.ShowtimeId == showtimeId)
                .Select(ts => ts.TheaterId)
                .ToListAsync();

            var cities = await _context.Cities
                .Where(c => c.RegionId == regionId && c.Cinemas!.Any(ci => ci.Theaters!.Any()))
                .Include(c => c.Cinemas!).ThenInclude(ci => ci.Theaters!)
                .ToListAsync();

            return cities.Select(c => new CityNodeDto
            {
                Id = c.Id,
                Name = c.Name,
                TotalCount = c.Cinemas!.SelectMany(ci => ci.Theaters!).Count(),
                SelectedCount = c.Cinemas!.SelectMany(ci => ci.Theaters!).Count(t => selectedIds.Contains(t.Id))
            }).ToList();
        }

        // Cấp 3: Cinema
        public async Task<List<CinemaNodeDto>> GetCinemasByCityAsync(int cityId, int showtimeId)
        {
            var selectedIds = await _context.TheaterShowtimes
                .Where(ts => ts.ShowtimeId == showtimeId)
                .Select(ts => ts.TheaterId)
                .ToListAsync();

            var cinemas = await _context.Cinemas
                .Where(ci => ci.CityId == cityId && ci.Theaters!.Any())
                .Include(ci => ci.Theaters!)
                .ToListAsync();

            return cinemas.Select(ci => new CinemaNodeDto
            {
                Id = ci.Id,
                Name = ci.Name,
                TotalCount = ci.Theaters!.Count,
                SelectedCount = ci.Theaters!.Count(t => selectedIds.Contains(t.Id))
            }).ToList();
        }

        // Cấp 4: Theater
        public async Task<List<TheaterNodeDto>> GetTheatersByCinemaAsync(int cinemaId, int showtimeId)
        {
            var selectedIds = await _context.TheaterShowtimes
                .Where(ts => ts.ShowtimeId == showtimeId)
                .Select(ts => ts.TheaterId)
                .ToListAsync();

            var theaters = await _context.Theaters
                .Where(t => t.CinemaId == cinemaId)
                .ToListAsync();

            return theaters.Select(t => new TheaterNodeDto
            {
                Id = t.Id,
                Name = t.Name,
                IsSelected = selectedIds.Contains(t.Id)
            }).ToList();
        }

        // Mở toàn bộ nhánh
        public async Task<List<RegionNodeDto>> GetFullTreeForShowtimeAsync(int showtimeId)
        {
            var selectedIds = await _context.TheaterShowtimes
                .Where(ts => ts.ShowtimeId == showtimeId)
                .Select(ts => ts.TheaterId)
                .ToListAsync();

            var regions = await _context.Regions
                .Include(r => r.Cities!)
                    .ThenInclude(c => c.Cinemas!)
                        .ThenInclude(ci => ci.Theaters!)
                .ToListAsync();

            var result = new List<RegionNodeDto>();

            foreach (var r in regions)
            {
                var regionDto = new RegionNodeDto { Id = r.Id, Name = r.Name };

                foreach (var c in r.Cities ?? Enumerable.Empty<City>())
                {
                    var cinemasWithTheaters = (c.Cinemas ?? Enumerable.Empty<Cinema>())
                        .Where(ci => (ci.Theaters ?? Enumerable.Empty<Theater>()).Any())
                        .ToList();

                    if (!cinemasWithTheaters.Any()) continue;

                    var cityDto = new CityNodeDto { Id = c.Id, Name = c.Name };

                    foreach (var ci in cinemasWithTheaters)
                    {
                        var cinemaDto = new CinemaNodeDto { Id = ci.Id, Name = ci.Name };

                        foreach (var t in ci.Theaters ?? Enumerable.Empty<Theater>())
                        {
                            cinemaDto.Theaters.Add(new TheaterNodeDto
                            {
                                Id = t.Id,
                                Name = t.Name,
                                IsSelected = selectedIds.Contains(t.Id)
                            });
                        }

                        cinemaDto.TotalCount = cinemaDto.Theaters.Count;
                        cinemaDto.SelectedCount = cinemaDto.Theaters.Count(th => th.IsSelected);
                        cityDto.Cinemas.Add(cinemaDto);
                    }

                    cityDto.TotalCount = cityDto.Cinemas.Sum(ci => ci.TotalCount);
                    cityDto.SelectedCount = cityDto.Cinemas.Sum(ci => ci.SelectedCount);
                    regionDto.Cities.Add(cityDto);
                }

                regionDto.TotalCount = regionDto.Cities.Sum(ct => ct.TotalCount);
                regionDto.SelectedCount = regionDto.Cities.Sum(ct => ct.SelectedCount);

                if (regionDto.TotalCount > 0)
                    result.Add(regionDto);
            }

            return result;
        }

        public async Task<List<RegionNodeDto>> GetCollapsedTreeForShowtimeAsync(int showtimeId)
            => await GetRegionsForShowtimeAsync(showtimeId);

        // Thêm / Xóa Theater
        public async Task<(bool Success, List<ConflictedTheaterDTO> Conflicts)> AddTheatersToShowtimeAsync(
            int showtimeId, List<int> theaterIds)
        {
            var showtime = await _context.Showtimes
                .FirstOrDefaultAsync(s => s.Id == showtimeId);

            if (showtime == null)
                throw new Exception($"Showtime {showtimeId} không tồn tại.");

            // 1️⃣ Kiểm tra theater nào đang bị trùng thời gian với showtime khác
            var conflicted = await GetConflictedTheatersAsync(
                theaterIds,
                showtime.Date,
                showtime.StartTime,
                showtime.EndTime,
                excludeShowtimeId: showtimeId
            );

            if (conflicted.Any())
            {
                // Trả về danh sách đụng độ để frontend hiển thị cảnh báo
                return (false, conflicted);
            }

            // 2️⃣ Lấy danh sách theater đã gán cho showtime này
            var existing = await _context.TheaterShowtimes
                .Where(ts => ts.ShowtimeId == showtimeId)
                .Select(ts => ts.TheaterId)
                .ToListAsync();

            // 3️⃣ Lọc ra những theater mới chưa được gán
            var newTheaters = theaterIds
                .Except(existing)
                .Select(id => new TheaterShowtime
                {
                    ShowtimeId = showtimeId,
                    TheaterId = id
                })
                .ToList();

            if (!newTheaters.Any())
                return (false, new List<ConflictedTheaterDTO>());

            // 4️⃣ Thêm mới theater-showtime
            _context.TheaterShowtimes.AddRange(newTheaters);
            await _context.SaveChangesAsync();

            return (true, new List<ConflictedTheaterDTO>());
        }

        public async Task<bool> RemoveTheatersFromShowtimeAsync(int showtimeId, List<int> theaterIds)
        {
            var entities = await _context.TheaterShowtimes
                .Where(ts => ts.ShowtimeId == showtimeId && theaterIds.Contains(ts.TheaterId))
                .ToListAsync();

            if (!entities.Any())
                return false;

            _context.TheaterShowtimes.RemoveRange(entities);
            await _context.SaveChangesAsync();
            return true;
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
    }
}
