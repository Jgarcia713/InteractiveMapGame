using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles;
using InteractiveMapGame.Data;
using InteractiveMapGame.Services;
using InteractiveMapGame.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

// Add authentication service
builder.Services.AddScoped<IAuthService, AuthService>();

// Add Entity Framework
builder.Services.AddDbContext<MapGameDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        // Fallback to user secrets for development
        connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
    }
    options.UseSqlite(connectionString);
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Interactive Map Game API", 
        Version = "v1",
        Description = "API for the Interactive Map Game with LLM integration and 360 video support"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Interactive Map Game API v1");
        c.RoutePrefix = "swagger";
    });
}

// Enable CORS
app.UseCors("AllowAll");

// Add admin authentication middleware
app.UseAdminAuth();

// Enable static files
app.UseDefaultFiles();

// Serve static files including GLB/GLTF/KTX2 with proper content types
var staticFileProvider = new FileExtensionContentTypeProvider();
staticFileProvider.Mappings[".glb"] = "model/gltf-binary";
staticFileProvider.Mappings[".gltf"] = "model/gltf+json";
staticFileProvider.Mappings[".ktx2"] = "image/ktx2";
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = staticFileProvider
});

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// Fallback to index.html for SPA routing
app.MapFallbackToFile("index.html");

app.Run();
