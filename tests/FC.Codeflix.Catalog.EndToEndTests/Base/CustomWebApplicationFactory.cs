using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Sdk;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;

// TStartup: Tipo da classe em que a API starta. Onde o setup é feito
public class CustomWebApplicationFactory<TStartup> 
    : WebApplicationFactory<TStartup>
    where TStartup : class 
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Altero o ambiente para o EndToEndTest
        builder.UseEnvironment("EndToEndTest"); // Mesmo nome que tá no appsettings do EndToEndTest na API
        // Encontro o serviço que seja o DbContext
        builder.ConfigureServices(services =>
        {
            // Configs para o banco em memória
            // var dbOptions = services.FirstOrDefault(x => x.ServiceType == typeof(DbContextOptions<CodeflixCatalogDbContext>));
            //
            // // Caso o serviço exista, ele é removido
            // if (dbOptions is not null)
            //     services.Remove(dbOptions);
            //
            //
            // // Adiciono um DbContext modificado para o meu teste end2end
            // services.AddDbContext<CodeflixCatalogDbContext>(options =>
            // {
            //     options.UseInMemoryDatabase("end2end-tests-db");
            // });
            
            // Para o banco MySQL do teste EndToEnd
            // Pego o ServiceProvider
            var serviceProvider = services.BuildServiceProvider();
            // Crio um escopo
            using var scope = serviceProvider.CreateScope();
            // Pega um CodeflixCatalogDbContext
            var context = scope.ServiceProvider.GetService<CodeflixCatalogDbContext>();
            
            // Vai lançar uma ArgumentNullException se o context for null
            ArgumentNullException.ThrowIfNull(context);

            // Deleta e cria um novo banco
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        });

        base.ConfigureWebHost(builder);
    }
}