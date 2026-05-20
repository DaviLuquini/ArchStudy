using ArchStudy.Clean;
using ArchStudy.Cqrs;
using ArchStudy.Ddd;
using ArchStudy.FeatureBased;
using ArchStudy.Hexagonal;
using ArchStudy.Host;
using ArchStudy.Mvc;
using ArchStudy.Onion;
using ArchStudy.VerticalSlice;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddMvcModule();
builder.Services.AddFeatureBasedModule();
builder.Services.AddVerticalSliceModule();
builder.Services.AddOnionModule();
builder.Services.AddHexagonalModule();
builder.Services.AddCleanModule();
builder.Services.AddDddModule();
builder.Services.AddCqrsModule();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseMiddleware<ExceptionMappingMiddleware>();

app.MapGet("/", () => Results.Ok(new
{
    name = "ArchStudy",
    description = "Same domain (bank accounts) implemented in 8 architectures",
    architectures = new[]
    {
        new { slug = "mvc",             prefix = "/mvc" },
        new { slug = "feature-based",   prefix = "/feature-based" },
        new { slug = "vertical-slice",  prefix = "/vertical-slice" },
        new { slug = "onion",           prefix = "/onion" },
        new { slug = "hexagonal",       prefix = "/hexagonal" },
        new { slug = "clean",           prefix = "/clean" },
        new { slug = "ddd",             prefix = "/ddd" },
        new { slug = "cqrs",            prefix = "/cqrs" },
    },
}));

app.MapMvcModule();
app.MapFeatureBasedModule();
app.MapVerticalSliceModule();
app.MapOnionModule();
app.MapHexagonalModule();
app.MapCleanModule();
app.MapDddModule();
app.MapCqrsModule();

await ModuleDatabaseInitializer.EnsureCreatedAsync(app.Services);

app.Run();

internal static class ModuleDatabaseInitializer
{
    public static async Task EnsureCreatedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        await MvcModule.EnsureDatabaseCreatedAsync(scope.ServiceProvider);
        await FeatureBasedModule.EnsureDatabaseCreatedAsync(scope.ServiceProvider);
        await VerticalSliceModule.EnsureDatabaseCreatedAsync(scope.ServiceProvider);
        await OnionModule.EnsureDatabaseCreatedAsync(scope.ServiceProvider);
        await HexagonalModule.EnsureDatabaseCreatedAsync(scope.ServiceProvider);
        await CleanModule.EnsureDatabaseCreatedAsync(scope.ServiceProvider);
        await DddModule.EnsureDatabaseCreatedAsync(scope.ServiceProvider);
        await CqrsModule.EnsureDatabaseCreatedAsync(scope.ServiceProvider);
    }
}
