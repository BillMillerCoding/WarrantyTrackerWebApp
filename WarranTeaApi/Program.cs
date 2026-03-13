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

builder.Services.AddIdentityApiEndpoints<User>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddHttpClient();
builder.Services.AddTransient<EmbeddingService>();
builder.Services.AddTransient<SearchService>();
builder.Services.AddTransient<BlobService>();
builder.Services.AddTransient<OcrService>();
builder.Services.AddScoped<ProductWarrantyService>();
builder.Services.AddScoped<WarrantyService>();
builder.Services.AddTransient<ProductService>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Apply migrations, ensure blob container, and seed product warranties on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var blobService = scope.ServiceProvider.GetRequiredService<BlobService>();
    await blobService.EnsureContainerExistsAsync();

    var pwService = scope.ServiceProvider.GetRequiredService<ProductWarrantyService>();

    var searchService = scope.ServiceProvider.GetRequiredService<SearchService>();
    await searchService.CreateIndexIfNotExistsAsync();

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/api/identity").MapIdentityApi<User>();
app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
