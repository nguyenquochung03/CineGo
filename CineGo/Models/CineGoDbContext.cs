using Microsoft.EntityFrameworkCore;

namespace CineGo.Models
{
    public class CineGoDbContext : DbContext
    {
        public CineGoDbContext(DbContextOptions<CineGoDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Region> Regions => Set<Region>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Cinema> Cinemas => Set<Cinema>();
        public DbSet<Theater> Theaters => Set<Theater>();
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Showtime> Showtimes => Set<Showtime>();
        public DbSet<ShowtimePrice> ShowtimePrices => Set<ShowtimePrice>();
        public DbSet<PricingRule> PricingRules => Set<PricingRule>();
        public DbSet<PricingRuleDay> PricingRuleDays => Set<PricingRuleDay>();
        public DbSet<PricingDetail> PricingDetails => Set<PricingDetail>();
        public DbSet<Seat> Seats => Set<Seat>();
        public DbSet<SeatStatus> SeatStatuses => Set<SeatStatus>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<PromoCode> PromoCodes => Set<PromoCode>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Quan hệ Region - City
            modelBuilder.Entity<City>()
                .HasOne(c => c.Region)
                .WithMany(r => r.Cities)
                .HasForeignKey(c => c.RegionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ City - Cinema
            modelBuilder.Entity<Cinema>()
                .HasOne(c => c.City)
                .WithMany(city => city.Cinemas)
                .HasForeignKey(c => c.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ Cinema - Theater
            modelBuilder.Entity<Theater>()
                .HasOne(t => t.Cinema)
                .WithMany(c => c.Theaters)
                .HasForeignKey(t => t.CinemaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ Theater - Showtime
            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.Theater)
                .WithMany(t => t.Showtimes)
                .HasForeignKey(s => s.TheaterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ Movie - Showtime
            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Showtimes)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ Movie - MoviePoster
            modelBuilder.Entity<MoviePoster>()
                .HasOne(mp => mp.Movie)
                .WithMany(m => m.Posters)
                .HasForeignKey(mp => mp.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ Movie - Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ User - Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ Showtime - ShowtimePrice
            modelBuilder.Entity<ShowtimePrice>()
                .HasOne(sp => sp.Showtime)
                .WithMany(s => s.ShowtimePrices)
                .HasForeignKey(sp => sp.ShowtimeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ PricingRule - Showtime
            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.PricingRule)
                .WithMany(pr => pr.Showtimes)
                .HasForeignKey(s => s.PricingRuleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ PricingRule - PricingRuleDay
            modelBuilder.Entity<PricingRuleDay>()
                .HasOne(prd => prd.PricingRule)
                .WithMany(pr => pr.ApplicableDays)
                .HasForeignKey(prd => prd.PricingRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ PricingRule - PricingDetail
            modelBuilder.Entity<PricingDetail>()
                .HasOne(pd => pd.PricingRule)
                .WithMany(pr => pr.PricingDetails)
                .HasForeignKey(pd => pd.PricingRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ Theater - Seat
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Theater)
                .WithMany(t => t.Seats)
                .HasForeignKey(s => s.TheaterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ Seat - SeatStatus
            modelBuilder.Entity<SeatStatus>()
                .HasOne(ss => ss.Seat)
                .WithMany(s => s.SeatStatuses)
                .HasForeignKey(ss => ss.SeatId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ Showtime - SeatStatus
            modelBuilder.Entity<SeatStatus>()
                .HasOne(ss => ss.Showtime)
                .WithMany(s => s.SeatStatuses)
                .HasForeignKey(ss => ss.ShowtimeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ User - SeatStatus
            modelBuilder.Entity<SeatStatus>()
                .HasOne(ss => ss.User)
                .WithMany(u => u.LockedSeats)
                .HasForeignKey(ss => ss.LockedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Quan hệ User - Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ Order - Showtime
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Showtime)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.ShowtimeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ Order - Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ Order - PromoCode
            modelBuilder.Entity<Order>()
                .HasOne(o => o.PromoCode)
                .WithMany(pc => pc.Orders)
                .HasForeignKey(o => o.PromoCodeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Quan hệ Order - OrderItem
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ OrderItem và Ticket
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.OrderItem)
                .WithOne(oi => oi.Ticket)
                .HasForeignKey<Ticket>(t => t.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);


            base.OnModelCreating(modelBuilder);
        }
    }
}
