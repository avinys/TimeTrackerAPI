using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TimeTrackerAPI.Middlewares
{
    public static class JwtCookieAuth
    {
        public static IServiceCollection AddJwtCookieAuth(this IServiceCollection services, IConfiguration config)
        {
            //Console.WriteLine("SECRET KEY: " + config["JwtSettings:SecretKey"]);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = config["JwtSettings:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = config["JwtSettings:Audience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"]))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["token"];
                            return Task.CompletedTask;
                        }
                    };
                });
            return services;
        }
    }
}
