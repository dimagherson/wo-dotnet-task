using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movies.Contracts.Repository;

namespace Movies.Repository
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return services
                .AddScoped<IMoviesRepository, OMDbMoviesRepository>()
                .AddScoped(x => configuration.GetSection("OMDbApiOptions").Get<OMDbApiOptions>())
                .AddScoped<IParser, JsonParser>();
        }
    }
}
