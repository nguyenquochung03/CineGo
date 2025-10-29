using CineGo.DTO;
using CineGo.DTO.Common;
using CineGo.DTO.Movie;
using CineGo.DTO.MoviePoster;
using CineGo.DTO.Review;
using CineGo.Models;
using CineGo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineGo.Services.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly CineGoDbContext _context;

        public MovieService(CineGoDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách phim theo trang
        public async Task<ApiResponse> GetPagedAsync(int page = 1, int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Movies.AsQueryable();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var movies = await query
                .Include(m => m.Posters)
                .Include(m => m.Reviews)
                .OrderBy(m => m.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MovieDTO
                {
                    Id = m.Id,
                    Title = m.Title,
                    Slug = m.Slug,
                    Runtime = m.Runtime,
                    Rating = m.Rating,
                    AgeLimit = m.AgeLimit,
                    ReleaseDate = m.ReleaseDate,
                    Synopsis = m.Synopsis,
                    TrailerUrl = m.TrailerUrl,
                    Posters = m.Posters.Select(p => new MoviePosterDTO
                    {
                        Id = p.Id,
                        Url = p.Url,
                        Order = p.Order
                    }).ToList(),
                    Reviews = m.Reviews.Select(r => new ReviewDTO
                    {
                        Id = r.Id,
                        Content = r.Content,
                        Rating = r.Rating
                    }).ToList()
                })
                .ToListAsync();

            var result = new PagedResult<MovieDTO>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = movies
            };

            return ApiResponse.SuccessResponse(result);
        }

        // Lấy phim theo ID
        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Posters)
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return ApiResponse.ErrorResponse(404, "Phim không tồn tại.");

            var dto = new MovieDTO
            {
                Id = movie.Id,
                Title = movie.Title,
                Slug = movie.Slug,
                Runtime = movie.Runtime,
                Rating = movie.Rating,
                AgeLimit = movie.AgeLimit,
                ReleaseDate = movie.ReleaseDate,
                Synopsis = movie.Synopsis,
                TrailerUrl = movie.TrailerUrl,
                Posters = movie.Posters?.Select(p => new MoviePosterDTO
                {
                    Id = p.Id,
                    Url = p.Url,
                    Order = p.Order
                }).ToList() ?? new List<MoviePosterDTO>(),
                Reviews = movie.Reviews?.Select(r => new ReviewDTO
                {
                    Id = r.Id,
                    Content = r.Content,
                    Rating = r.Rating
                }).ToList() ?? new List<ReviewDTO>()
            };

            return ApiResponse.SuccessResponse(dto);
        }

        // Tạo phim mới
        public async Task<ApiResponse> CreateAsync(MovieCreateUpdateDTO dto)
        {
            if (await _context.Movies.AnyAsync(m => m.Slug == dto.Slug))
                return ApiResponse.ErrorResponse(400, "Slug phim đã tồn tại.");

            var movie = new Movie
            {
                Title = dto.Title,
                Slug = dto.Slug,
                Runtime = dto.Runtime,
                Rating = dto.Rating,
                AgeLimit = dto.AgeLimit,
                ReleaseDate = dto.ReleaseDate,
                Synopsis = dto.Synopsis,
                TrailerUrl = dto.TrailerUrl
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            if (dto.Poster != null && dto.Poster.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{dto.Poster.FileName}";
                var filePath = Path.Combine("wwwroot/uploads/movies", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Poster.CopyToAsync(stream);
                }

                var poster = new MoviePoster
                {
                    MovieId = movie.Id,
                    Url = $"/uploads/movies/{fileName}",
                    Order = 0
                };
                _context.MoviePosters.Add(poster);
                await _context.SaveChangesAsync();
            }

            var resultDto = new MovieDTO
            {
                Id = movie.Id,
                Title = movie.Title,
                Slug = movie.Slug,
                Runtime = movie.Runtime,
                Rating = movie.Rating,
                AgeLimit = movie.AgeLimit,
                ReleaseDate = movie.ReleaseDate,
                Synopsis = movie.Synopsis,
                TrailerUrl = movie.TrailerUrl,
                Posters = movie.Posters?.Select(p => new MoviePosterDTO
                {
                    Id = p.Id,
                    Url = p.Url,
                    Order = p.Order
                }).ToList() ?? new List<MoviePosterDTO>(),
                Reviews = new List<ReviewDTO>()
            };

            return ApiResponse.SuccessResponse(resultDto, "Tạo phim thành công.");
        }

        // Cập nhật phim
        public async Task<ApiResponse> UpdateAsync(int id, MovieCreateUpdateDTO dto)
        {
            var existing = await _context.Movies
                .Include(m => m.Posters)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (existing == null)
                return ApiResponse.ErrorResponse(404, "Phim không tồn tại.");

            if (await _context.Movies.AnyAsync(m => m.Slug == dto.Slug && m.Id != id))
                return ApiResponse.ErrorResponse(400, "Slug phim đã tồn tại.");

            existing.Title = dto.Title;
            existing.Slug = dto.Slug;
            existing.Runtime = dto.Runtime;
            existing.Rating = dto.Rating;
            existing.AgeLimit = dto.AgeLimit;
            existing.ReleaseDate = dto.ReleaseDate;
            existing.Synopsis = dto.Synopsis;
            existing.TrailerUrl = dto.TrailerUrl;

            // Xử lý poster mới
            if (dto.Poster != null && dto.Poster.Length > 0)
            {
                // Xóa poster cũ nếu có
                var oldPoster = existing.Posters.FirstOrDefault(p => p.Order == 0);
                if (oldPoster != null)
                {
                    var oldPath = Path.Combine("wwwroot", oldPoster.Url.TrimStart('/'));
                    if (File.Exists(oldPath)) File.Delete(oldPath);
                    _context.MoviePosters.Remove(oldPoster);
                }

                // Lưu poster mới
                var fileName = $"{Guid.NewGuid()}_{dto.Poster.FileName}";
                var filePath = Path.Combine("wwwroot/uploads/movies", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Poster.CopyToAsync(stream);
                }

                var poster = new MoviePoster
                {
                    MovieId = existing.Id,
                    Url = $"/uploads/movies/{fileName}",
                    Order = 0
                };
                _context.MoviePosters.Add(poster);
            }

            await _context.SaveChangesAsync();

            var resultDto = new MovieDTO
            {
                Id = existing.Id,
                Title = existing.Title,
                Slug = existing.Slug,
                Runtime = existing.Runtime,
                Rating = existing.Rating,
                AgeLimit = existing.AgeLimit,
                ReleaseDate = existing.ReleaseDate,
                Synopsis = existing.Synopsis,
                TrailerUrl = existing.TrailerUrl,
                Posters = existing.Posters?.Select(p => new MoviePosterDTO
                {
                    Id = p.Id,
                    Url = p.Url,
                    Order = p.Order
                }).ToList() ?? new List<MoviePosterDTO>(),
                Reviews = existing.Reviews?.Select(r => new ReviewDTO
                {
                    Id = r.Id,
                    Content = r.Content,
                    Rating = r.Rating
                }).ToList() ?? new List<ReviewDTO>()
            };

            return ApiResponse.SuccessResponse(resultDto, "Cập nhật phim thành công.");
        }

        // Xóa phim
        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Showtimes)
                .Include(m => m.Posters)
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return ApiResponse.ErrorResponse(404, "Phim không tồn tại.");

            if (movie.Showtimes != null && movie.Showtimes.Any())
                return ApiResponse.ErrorResponse(400, "Không thể xóa phim có suất chiếu.");

            if (movie.Posters != null) _context.MoviePosters.RemoveRange(movie.Posters);
            if (movie.Reviews != null) _context.Reviews.RemoveRange(movie.Reviews);

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(null, "Xóa phim thành công.");
        }

        public async Task<ApiResponse> SearchByTitleAsync(string title, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(title))
                return ApiResponse.ErrorResponse(400, "Tên phim không được để trống.");

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Movies
                .Where(m => m.Title.Contains(title))
                .OrderBy(m => m.Title);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var movies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MovieDTO
                {
                    Id = m.Id,
                    Title = m.Title,
                    Slug = m.Slug,
                    Runtime = m.Runtime,
                    Rating = m.Rating,
                    AgeLimit = m.AgeLimit,
                    ReleaseDate = m.ReleaseDate,
                    Synopsis = m.Synopsis,
                    TrailerUrl = m.TrailerUrl,
                    Posters = m.Posters.Select(p => new MoviePosterDTO
                    {
                        Id = p.Id,
                        Url = p.Url,
                        Order = p.Order
                    }).ToList(),
                    Reviews = m.Reviews.Select(r => new ReviewDTO
                    {
                        Id = r.Id,
                        Content = r.Content,
                        Rating = r.Rating
                    }).ToList()
                })
                .ToListAsync();

            var result = new PagedResult<MovieDTO>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = movies
            };

            return ApiResponse.SuccessResponse(result);
        }

        public async Task<ApiResponse> GetNowShowingAsync(int page = 1, int pageSize = 8)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var today = DateTime.Today;

            var query = _context.Movies
                .Where(m => m.ReleaseDate <= today && m.Showtimes.Any(s => s.Date >= today))
                .OrderBy(m => m.Title)
                .AsQueryable();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var movies = await query
                .Include(m => m.Posters)
                .Include(m => m.Reviews)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MovieDTO
                {
                    Id = m.Id,
                    Title = m.Title,
                    Slug = m.Slug,
                    Runtime = m.Runtime,
                    Rating = m.Rating,
                    AgeLimit = m.AgeLimit,
                    ReleaseDate = m.ReleaseDate,
                    Synopsis = m.Synopsis,
                    TrailerUrl = m.TrailerUrl,
                    Posters = m.Posters.Select(p => new MoviePosterDTO
                    {
                        Id = p.Id,
                        Url = p.Url,
                        Order = p.Order
                    }).ToList(),
                    Reviews = m.Reviews.Select(r => new ReviewDTO
                    {
                        Id = r.Id,
                        Content = r.Content,
                        Rating = r.Rating
                    }).ToList()
                })
                .ToListAsync();

            var result = new PagedResult<MovieDTO>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = movies
            };

            return ApiResponse.SuccessResponse(result);
        }

        public async Task<ApiResponse> GetComingSoonAsync(int page = 1, int pageSize = 8)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var today = DateTime.Today;

            var query = _context.Movies
                .Where(m => m.ReleaseDate > today)
                .OrderBy(m => m.ReleaseDate)
                .AsQueryable();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var movies = await query
                .Include(m => m.Posters)
                .Include(m => m.Reviews)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MovieDTO
                {
                    Id = m.Id,
                    Title = m.Title,
                    Slug = m.Slug,
                    Runtime = m.Runtime,
                    Rating = m.Rating,
                    AgeLimit = m.AgeLimit,
                    ReleaseDate = m.ReleaseDate,
                    Synopsis = m.Synopsis,
                    TrailerUrl = m.TrailerUrl,
                    Posters = m.Posters.Select(p => new MoviePosterDTO
                    {
                        Id = p.Id,
                        Url = p.Url,
                        Order = p.Order
                    }).ToList(),
                    Reviews = m.Reviews.Select(r => new ReviewDTO
                    {
                        Id = r.Id,
                        Content = r.Content,
                        Rating = r.Rating
                    }).ToList()
                })
                .ToListAsync();

            var result = new PagedResult<MovieDTO>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = movies
            };

            return ApiResponse.SuccessResponse(result);
        }

        public async Task<ApiResponse> GetByDateAndCinemaAsync(DateTime date, int cinemaId)
        {
            // Lấy tất cả phim có suất chiếu trong rạp và ngày chỉ định
            var movies = await _context.Movies
                .Include(m => m.Showtimes)
                    .ThenInclude(s => s.TheaterShowtimes)
                        .ThenInclude(ts => ts.Theater)
                .Where(m => m.Showtimes.Any(s =>
                    s.Date.Date == date.Date &&
                    s.TheaterShowtimes.Any(ts => ts.Theater.CinemaId == cinemaId)))
                .Distinct()
                .ToListAsync();

            if (!movies.Any())
            {
                return ApiResponse.ErrorResponse(404, "Không có phim nào chiếu ở rạp này trong ngày đã chọn.");
            }

            var movieDtos = movies.Select(m => new
            {
                m.Id,
                m.Title,
                m.ReleaseDate,
                m.Rating,
                m.AgeLimit,
                m.Runtime,
                m.Slug
            }).ToList();

            return ApiResponse.SuccessResponse(movieDtos);
        }
    }
}