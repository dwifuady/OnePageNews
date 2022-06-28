using System.Reflection;
using Application.Scrapper;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddSingleton<IScrapper, DetikScrapper>();
        services.AddSingleton<IScrapper, KompasScrapper>();
        services.AddHttpClient<BaseScrapper>();

        return services;
    }
}