using FC.Codeflix.Catalog.Api.Filters;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class ControllersConfiguration
{
    public static IServiceCollection AddAndConfiguraControllers(this IServiceCollection services)
    {
        services.AddControllers(options => options.Filters.Add(typeof(ApiGlobalExeptionFilter)));
        services.AddDocumentation();

        return services;
    }

    private static IServiceCollection AddDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static WebApplication UseDocumentation(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }

}