using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SignBridge.Application.Interfaces;
using SignBridge.Infrastructure.Persistence;
using SignBridge.Infrastructure.Security;
using SignBridge.Infrastructure.Services;

namespace SignBridge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var provider = configuration["Database:Provider"] ?? "Sqlite";

        if (provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
        {
            var connectionString = configuration.GetConnectionString("Postgres")
                ?? throw new InvalidOperationException("Postgres connection string is missing.");

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("Sqlite")
                ?? "Data Source=signbridge.db";

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString));
        }

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<JwtTokenService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILearningService, LearningService>();
        services.AddScoped<IParentService, ParentService>();
        services.AddScoped<IContentService, ContentService>();

        return services;
    }
}
