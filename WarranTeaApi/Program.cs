using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WarranTeaApi.Data;
using WarranTeaApi.Models;
using WarranTeaApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Name = "WarranTea.Auth";
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.SlidingExpiration = true;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

builder.Services.AddHttpClient();
builder.Services.AddTransient<EmbeddingService>();
builder.Services.AddTransient<SearchService>();
builder.Services.AddTransient<BlobService>();
builder.Services.AddTransient<OcrService>();
builder.Services.AddScoped<ProductWarrantyService>();
builder.Services.AddScoped<WarrantyService>();
builder.Services.AddTransient<ProductService>();
builder.Services.AddTransient<AiService>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                  "http://localhost:3000",
                  "https://agreeable-rock-06b51d41e.1.azurestaticapps.net")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Apply migrations, ensure blob container, and seed product warranties on startup
using (var scope = app.Services.CreateScope())
try
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    Console.WriteLine("Database migration completed successfully.");

    var blobService = scope.ServiceProvider.GetRequiredService<BlobService>();
    await blobService.EnsureContainerExistsAsync();
    Console.WriteLine("Blob container ensured.");

    var pwService = scope.ServiceProvider.GetRequiredService<ProductWarrantyService>();

    var searchService = scope.ServiceProvider.GetRequiredService<SearchService>();
    await searchService.CreateIndexIfNotExistsAsync();
    Console.WriteLine("Search index ensured.");

    // Only seed if the table is empty (first run after migration)
    if (!await db.ProductWarranties.AnyAsync())
    {
        var seedData = ProductWarrantySeedData.GetAll();
        Console.WriteLine($"Seeding {seedData.Count} product warranties...");

        int count = 0;
        foreach (var dto in seedData)
        {
            await pwService.CreateAsync(dto);
            count++;
            Console.WriteLine($"  Seeded ({count}/{seedData.Count}): {dto.CoverageType} for product {dto.ProductId}");
            await Task.Delay(1500); // Pace requests to avoid Azure AI Vision 429s
        }

        Console.WriteLine("Product warranty seeding complete.");
    }
    else
    {
        Console.WriteLine("Product warranties already seeded — skipping.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Startup initialization error: {ex}");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/health", async (AppDbContext db) =>
{
    var result = new Dictionary<string, string>();
    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        result["database"] = canConnect ? "connected" : "cannot connect";
        if (canConnect)
        {
            var productCount = await db.Products.CountAsync();
            var warrantyCount = await db.ProductWarranties.CountAsync();
            result["products"] = productCount.ToString();
            result["productWarranties"] = warrantyCount.ToString();
        }
    }
    catch (Exception ex)
    {
        result["database"] = $"error: {ex.Message}";
    }
    result["connectionString"] = string.IsNullOrEmpty(
        db.Database.GetConnectionString()) ? "MISSING" : "set (hidden)";
    return Results.Ok(result);
});

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
