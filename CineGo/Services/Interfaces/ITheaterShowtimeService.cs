using CineGo.DTO.Helper;
using CineGo.DTO.TheaterTree;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CineGo.Services.Interfaces
{
    public interface ITheaterShowtimeService
    {
        Task<List<RegionNodeDto>> GetRegionsForShowtimeAsync(int showtimeId);
        Task<List<CityNodeDto>> GetCitiesByRegionAsync(int regionId, int showtimeId);
        Task<List<CinemaNodeDto>> GetCinemasByCityAsync(int cityId, int showtimeId);
        Task<List<TheaterNodeDto>> GetTheatersByCinemaAsync(int cinemaId, int showtimeId);

        Task<List<RegionNodeDto>> GetFullTreeForShowtimeAsync(int showtimeId);
        Task<List<RegionNodeDto>> GetCollapsedTreeForShowtimeAsync(int showtimeId);

        Task<(bool Success, List<ConflictedTheaterDTO> Conflicts)> AddTheatersToShowtimeAsync(int showtimeId, List<int> theaterIds);
        Task<bool> RemoveTheatersFromShowtimeAsync(int showtimeId, List<int> theaterIds);
    }
}
