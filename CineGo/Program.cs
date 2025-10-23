using CineGo.Extensions;
using CineGo.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace CineGo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(120);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            // Add DbContext
            builder.Services.AddDbContext<CineGoDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CineGoConnection")));

            // JWT
            var jwtSecret = builder.Configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtSecret))
                throw new ArgumentNullException("JWT secret key chưa được cấu hình.");

            builder.Services.AddJwtAuthentication(jwtSecret);

            // Custom Services
            builder.Services.AddCustomServices();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler($"/Error/HandleError/{404}");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value?.ToLower();

                if (!path.Contains("/admin/adminauth/login"))
                {
                    var token = context.Request.Cookies["jwt"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Request.Headers["Authorization"] = $"Bearer {token}";
                    }
                }

                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            // Middleware bắt 404
            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.HasStarted)
                    return;

                var path = context.Request.Path;
                var statusCode = context.Response.StatusCode;

                if (statusCode == 404)
                {
                    Console.WriteLine($"404 detected. Path={path}, StatusCode={statusCode}");

                    if (path.StartsWithSegments("/Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.Redirect($"/Admin/Error/{statusCode}");
                    }
                    else
                    {
                        context.Response.Redirect($"/Error/{statusCode}");
                    }
                }
            });

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 401)
                {
                    context.Response.Cookies.Delete("jwt");
                }
            });

            // Middleware kiểm tra khi người dùng truy cập /Admin
            app.MapGet("/Admin", async context =>
            {
                var user = context.User;
                var isAuthenticated = user?.Identity?.IsAuthenticated ?? false;

                if (isAuthenticated)
                    context.Response.Redirect("/Admin/Dashboard/Index");
                else
                    context.Response.Redirect("/Admin/AdminAuth/Login");

                await Task.CompletedTask;
            });


            // Cấu hình route cho Areas (Admin)
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            // Route mặc định cho người dùng (Client)
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=MovieList}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
