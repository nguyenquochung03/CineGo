using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CineGo.Extensions
{
    public static class JwtServiceExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, string secretKey)
        {
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("Khóa bí mật JWT phải được cung cấp.", nameof(secretKey));

            var key = Encoding.ASCII.GetBytes(secretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Token error: " + context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        // Nếu đang ở login page → không redirect
                        var path = context.Request.Path.Value?.ToLower();
                        if (path.Contains("/admin/adminauth/login"))
                            return Task.CompletedTask;

                        if (context.Request.Cookies.ContainsKey("jwt"))
                            context.Response.Cookies.Delete("jwt");

                        context.Response.Redirect("/Admin/AdminAuth/Login?expired=true");
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}
