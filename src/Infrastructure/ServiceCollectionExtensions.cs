using System.Reflection;
using Application.Scraper;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddSingleton<IScraper, DetikScraper>();
        services.AddSingleton<IScraper, GeneralScraper>();
        services.AddHttpClient<BaseScraper>();

        return services;
    }
}