using FluentValidation;
using LevelUp.BLL.Common;
using Microsoft.Extensions.DependencyInjection;

namespace LevelUp.BLL.Extentions.DependencyEnjection;

public static class BusinessLogicServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        var assembly = typeof(Result).Assembly;

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(c => c.Where(t => t.Name.EndsWith("Service")))
                .AsMatchingInterface()
                .WithScopedLifetime());

        services.AddAutoMapper(x => x.AddMaps(assembly));

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
