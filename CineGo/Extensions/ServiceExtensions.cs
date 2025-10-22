using CineGo.Services.Interfaces;
using CineGo.Services.Implementations;
using CineGo.Services;

namespace CineGo.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<ICinemaService, CinemaService>();
            services.AddScoped<ITheaterService, TheaterService>();
            services.AddScoped<ISeatService, SeatService>();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IPricingRuleService, PricingRuleService>();
            services.AddScoped<IPricingRuleDayService, PricingRuleDayService>();
            services.AddScoped<IPricingDetailService, PricingDetailService>();
            services.AddScoped<IShowtimeService, ShowtimeService>();
            services.AddScoped<IShowtimePriceService, ShowtimePriceService>();
            services.AddScoped<ITheaterShowtimeService, TheaterShowtimeService>();
            services.AddScoped<IPromoCodeService, PromoCodeService>();
        }
    }
}
