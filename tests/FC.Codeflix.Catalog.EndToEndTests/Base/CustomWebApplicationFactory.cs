using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;

// TStartup: Tipo da classe em que a API starta. Onde o setup é feito
public class CustomWebApplicationFactory<TStartup> 
    : WebApplicationFactory<TStartup>
    where TStartup : class 
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Encontro o serviço que seja o DbContext
        builder.ConfigureServices(services =>
        {
            var dbOptions = services.FirstOrDefault(x => x.ServiceType == typeof(DbContextOptions<CodeflixCatalogDbContext>));
            
            // Caso o serviço exista, ele é removido
            if (dbOptions is not null)
                services.Remove(dbOptions);


            // Adiciono um DbContext modificado para o meu teste end2end
            services.AddDbContext<CodeflixCatalogDbContext>(options =>
            {
                options.UseInMemoryDatabase("end2end-tests-db");
            });
        });

        base.ConfigureWebHost(builder);
    }
}