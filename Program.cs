using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.HttpOverrides;
using InteractiveMapGame.Data;

var builder = WebApplication.CreateBuilder(args);

// Add forwarded headers middleware to handle reverse proxy scenarios
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto;
    // Honor path base from reverse proxy
    options.ForwardedPrefixHeaderName = "X-Forwarded-Prefix";
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddHttpClient();

// Add Entity Framework
builder.Services.AddDbContext<MapGameDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        // Fallback to user secrets for development
        connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
    }
    options.UseSqlServer(connectionString);
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

// Use forwarded headers middleware (must be early in pipeline)
app.UseForwardedHeaders();

// Configure path base from environment variable
var pathBase = builder.Configuration["ASPNETCORE_PATHBASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

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

// Enable static files
app.UseDefaultFiles();

// Serve static files including GLB/GLTF/KTX2/PLY with proper content types
var staticFileProvider = new FileExtensionContentTypeProvider();
staticFileProvider.Mappings[".glb"] = "model/gltf-binary";
staticFileProvider.Mappings[".gltf"] = "model/gltf+json";
staticFileProvider.Mappings[".ktx2"] = "image/ktx2";
staticFileProvider.Mappings[".ply"] = "application/octet-stream"; // PLY files for Gaussian splats
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = staticFileProvider
});

app.UseRouting();

app.MapControllers();

// Fallback to index.html for SPA routing
app.MapFallbackToFile("index.html");

// Apply database migrations and schema updates on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MapGameDbContext>();
    try
    {
        // Apply any pending migrations automatically
        // This ensures the database schema is up-to-date when the app starts
        dbContext.Database.Migrate();
        Console.WriteLine("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        // Log but don't fail startup if migrations fail
        // This allows the app to start even if the database is temporarily unavailable
        Console.WriteLine($"Warning: Could not apply database migrations: {ex.Message}");
    }
    
    try
    {
        // Update InteractionLogs columns to nvarchar(max) if not already
        // nvarchar(2000) has max_length = 4000, nvarchar(max) has max_length = -1
        dbContext.Database.ExecuteSqlRaw(@"
            IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InteractionLogs') AND name = 'LLMPrompt' AND max_length = 4000)
            BEGIN
                ALTER TABLE [InteractionLogs] ALTER COLUMN [LLMPrompt] nvarchar(max) NULL;
            END
            IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InteractionLogs') AND name = 'LLMResponse' AND max_length = 4000)
            BEGIN
                ALTER TABLE [InteractionLogs] ALTER COLUMN [LLMResponse] nvarchar(max) NULL;
            END
        ");
    }
    catch (Exception ex)
    {
        // Log but don't fail startup if column doesn't exist or update fails
        Console.WriteLine($"Warning: Could not update database columns: {ex.Message}");
    }
}

app.Run();
