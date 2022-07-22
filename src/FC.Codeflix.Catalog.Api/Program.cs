using FC.Codeflix.Catalog.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddUseCases()
    .AddAndConfiguraControllers();


var app = builder.Build();

app.UseDocumentation();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


// Para o BaseFixture do teste E2E
public partial class Program {}