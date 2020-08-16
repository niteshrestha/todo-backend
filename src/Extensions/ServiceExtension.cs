using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Todo.Data.Identity;
using Todo.Services;

namespace Todo.API.Extensions
{
    public static class ServiceExtension
    {
        public static void ConfigureMongoDb(this IServiceCollection services)
        {
            services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole>
                (options =>
                {
                    options.Password.RequiredLength = 12;
                }, identityOptions =>
                {
                    identityOptions.ConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
                });
        }

        public static void ConfigureJWT(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                    ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")))
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Todo API",
                    Version = "v1"
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
        }

        public static void ConfigureDI(this IServiceCollection services)
        {
            services.AddSingleton<IItem, ItemService>();
        }
    }
}
