using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WarranTeaApi.Models;

namespace WarranTeaApi.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public const string AliceId = "a1111111-1111-1111-1111-111111111111";
    public const string BobId = "b2222222-2222-2222-2222-222222222222";
    public const string CarolId = "c3333333-3333-3333-3333-333333333333";
    public const string AdminId = "d4444444-4444-4444-4444-444444444444";
    public const string AdminRoleId = "e5555555-5555-5555-5555-555555555555";

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductWarranty> ProductWarranties { get; set; }
    public DbSet<Warranty> Warranties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Warranty>(entity =>
        {
            entity.HasOne(w => w.User)
                  .WithMany(u => u.Warranties)
                  .HasForeignKey(w => w.UserId);

            entity.HasOne(w => w.Product)
                  .WithMany(p => p.Warranties)
                  .HasForeignKey(w => w.ProductId);
        });

        modelBuilder.Entity<ProductWarranty>(entity =>
        {
            entity.HasOne(pw => pw.Product)
                  .WithMany(p => p.ProductWarranties)
                  .HasForeignKey(pw => pw.ProductId);
        });

        SeedRolesAndUsers(modelBuilder);
        SeedProducts(modelBuilder);
        SeedWarranties(modelBuilder);
    }

    private static void SeedRolesAndUsers(ModelBuilder mb)
    {
        // Seed Admin role
        mb.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = AdminRoleId, Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = AdminRoleId }
        );

        var hasher = new PasswordHasher<User>();

        var alice = new User { Id = AliceId, UserName = "alice@warrantea.com", NormalizedUserName = "ALICE@WARRANTEA.COM", Email = "alice@warrantea.com", NormalizedEmail = "ALICE@WARRANTEA.COM", EmailConfirmed = true, Name = "Alice Johnson", SecurityStamp = "ALICE_STAMP", ConcurrencyStamp = AliceId };
        alice.PasswordHash = hasher.HashPassword(alice, "Alice1!");

        var bob = new User { Id = BobId, UserName = "bob@warrantea.com", NormalizedUserName = "BOB@WARRANTEA.COM", Email = "bob@warrantea.com", NormalizedEmail = "BOB@WARRANTEA.COM", EmailConfirmed = true, Name = "Bob Smith", SecurityStamp = "BOB_STAMP", ConcurrencyStamp = BobId };
        bob.PasswordHash = hasher.HashPassword(bob, "Bobob1!");

        var carol = new User { Id = CarolId, UserName = "carol@warrantea.com", NormalizedUserName = "CAROL@WARRANTEA.COM", Email = "carol@warrantea.com", NormalizedEmail = "CAROL@WARRANTEA.COM", EmailConfirmed = true, Name = "Carol Davis", SecurityStamp = "CAROL_STAMP", ConcurrencyStamp = CarolId };
        carol.PasswordHash = hasher.HashPassword(carol, "Carol1!");

        var admin = new User { Id = AdminId, UserName = "admin@warrantea.com", NormalizedUserName = "ADMIN@WARRANTEA.COM", Email = "admin@warrantea.com", NormalizedEmail = "ADMIN@WARRANTEA.COM", EmailConfirmed = true, Name = "Admin User", SecurityStamp = "ADMIN_STAMP", ConcurrencyStamp = AdminId };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin1!");

        mb.Entity<User>().HasData(alice, bob, carol, admin);

        // Assign Admin role
        mb.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { UserId = AdminId, RoleId = AdminRoleId }
        );
    }

    private static void SeedProducts(ModelBuilder mb)
    {
        mb.Entity<Product>().HasData(
            new Product { Id = 1,  Name = "MacBook Pro 16\"",                Brand = "Apple",       Category = "Laptops",            Description = "Apple's flagship professional laptop." },
            new Product { Id = 2,  Name = "Galaxy S25 Ultra",                Brand = "Samsung",     Category = "Smartphones",         Description = "Samsung's premium flagship smartphone." },
            new Product { Id = 3,  Name = "Dyson V15 Detect",                Brand = "Dyson",       Category = "Home Appliances",     Description = "Cordless vacuum with laser dust detection." },
            new Product { Id = 4,  Name = "Sony WH-1000XM5",                Brand = "Sony",        Category = "Audio",               Description = "Industry-leading noise cancelling headphones." },
            new Product { Id = 5,  Name = "LG C4 65\" OLED TV",             Brand = "LG",          Category = "TVs",                 Description = "65-inch OLED 4K smart TV." },
            new Product { Id = 6,  Name = "KitchenAid Artisan Stand Mixer",  Brand = "KitchenAid",  Category = "Kitchen Appliances",  Description = "Iconic tilt-head stand mixer." },
            new Product { Id = 7,  Name = "Dell XPS 15",                     Brand = "Dell",        Category = "Laptops",             Description = "Premium 15-inch ultrabook." },
            new Product { Id = 8,  Name = "Bose QuietComfort Ultra",         Brand = "Bose",        Category = "Audio",               Description = "Premium noise cancelling headphones with spatial audio." },
            new Product { Id = 9,  Name = "iPad Pro 13\"",                   Brand = "Apple",       Category = "Tablets",             Description = "Apple's most advanced tablet with M4 chip." },
            new Product { Id = 10, Name = "PlayStation 5 Pro",               Brand = "Sony",        Category = "Gaming",              Description = "Next-gen gaming console with enhanced GPU." },
            new Product { Id = 11, Name = "Roomba j9+",                      Brand = "iRobot",      Category = "Home Appliances",     Description = "Self-emptying robot vacuum with obstacle avoidance." },
            new Product { Id = 12, Name = "Canon EOS R6 Mark II",            Brand = "Canon",       Category = "Cameras",             Description = "Full-frame mirrorless camera with 24.2 MP sensor." },
            new Product { Id = 13, Name = "Samsung 990 Pro 2TB SSD",         Brand = "Samsung",     Category = "Storage",             Description = "PCIe 4.0 NVMe M.2 solid state drive." },
            new Product { Id = 14, Name = "Breville Barista Express",        Brand = "Breville",    Category = "Kitchen Appliances",  Description = "Semi-automatic espresso machine with built-in grinder." },
            new Product { Id = 15, Name = "LG WashTower",                    Brand = "LG",          Category = "Home Appliances",     Description = "Single-unit washer and dryer tower with AI technology." },
            new Product { Id = 16, Name = "Nintendo Switch 2",               Brand = "Nintendo",    Category = "Gaming",              Description = "Hybrid home and portable gaming console." },
            new Product { Id = 17, Name = "Sonos Arc Soundbar",              Brand = "Sonos",       Category = "Audio",               Description = "Premium Dolby Atmos soundbar for home theater." },
            new Product { Id = 18, Name = "HP Spectre x360 16\"",            Brand = "HP",          Category = "Laptops",             Description = "Premium 2-in-1 convertible laptop." },
            new Product { Id = 19, Name = "GoPro Hero 12 Black",             Brand = "GoPro",       Category = "Cameras",             Description = "Waterproof action camera with HyperSmooth stabilization." },
            new Product { Id = 20, Name = "Whirlpool French Door Refrigerator", Brand = "Whirlpool", Category = "Kitchen Appliances", Description = "36-inch counter-depth French door refrigerator." }
        );
    }

    private static void SeedWarranties(ModelBuilder mb)
    {
        // Alice – Products: 1,2,3,4,5,9,10,12,14,17
        mb.Entity<Warranty>().HasData(
            new Warranty { Id = 1,  UserId = AliceId, ProductId = 1,  ProductName = "MacBook Pro 16\"",        Brand = "Apple",    Category = "Laptops",           CoverageType = "Extended",     PurchaseDate = new DateTime(2025, 1, 15), ExpirationDate = new DateTime(2028, 1, 15), Status = "active",        Notes = "AppleCare+ purchased at time of sale." },
            new Warranty { Id = 2,  UserId = AliceId, ProductId = 2,  ProductName = "Galaxy S25 Ultra",        Brand = "Samsung",  Category = "Smartphones",       CoverageType = "Manufacturer", PurchaseDate = new DateTime(2025, 3, 1),  ExpirationDate = new DateTime(2026, 3, 1),  Status = "active",        Notes = "Carrier-purchased device." },
            new Warranty { Id = 3,  UserId = AliceId, ProductId = 3,  ProductName = "Dyson V15 Detect",        Brand = "Dyson",    Category = "Home Appliances",   CoverageType = "Manufacturer", PurchaseDate = new DateTime(2024, 6, 20), ExpirationDate = new DateTime(2026, 6, 20), Status = "active",        Notes = "Registered on Dyson website." },
            new Warranty { Id = 4,  UserId = AliceId, ProductId = 4,  ProductName = "Sony WH-1000XM5",        Brand = "Sony",     Category = "Audio",             CoverageType = "Extended",     PurchaseDate = new DateTime(2024, 11, 25),ExpirationDate = new DateTime(2026, 11, 25),Status = "active",        Notes = "Best Buy Geek Squad protection." },
            new Warranty { Id = 5,  UserId = AliceId, ProductId = 5,  ProductName = "LG C4 65\" OLED TV",     Brand = "LG",       Category = "TVs",               CoverageType = "Extended",     PurchaseDate = new DateTime(2024, 12, 26),ExpirationDate = new DateTime(2027, 12, 26),Status = "active",        Notes = "Post-holiday purchase. Wall mounted." },
            new Warranty { Id = 6,  UserId = AliceId, ProductId = 9,  ProductName = "iPad Pro 13\"",           Brand = "Apple",    Category = "Tablets",           CoverageType = "Extended",     PurchaseDate = new DateTime(2025, 5, 10), ExpirationDate = new DateTime(2027, 5, 10), Status = "active",        Notes = "Uses Magic Keyboard accessory." },
            new Warranty { Id = 7,  UserId = AliceId, ProductId = 10, ProductName = "PlayStation 5 Pro",       Brand = "Sony",     Category = "Gaming",            CoverageType = "Manufacturer", PurchaseDate = new DateTime(2025, 2, 14), ExpirationDate = new DateTime(2026, 2, 14), Status = "expiring-soon", Notes = "Gift from spouse." },
            new Warranty { Id = 8,  UserId = AliceId, ProductId = 12, ProductName = "Canon EOS R6 Mark II",   Brand = "Canon",    Category = "Cameras",           CoverageType = "Extended",     PurchaseDate = new DateTime(2024, 8, 5),  ExpirationDate = new DateTime(2027, 8, 5),  Status = "active",        Notes = "Canon CarePAK PLUS registered." },
            new Warranty { Id = 9,  UserId = AliceId, ProductId = 14, ProductName = "Breville Barista Express",Brand = "Breville",  Category = "Kitchen Appliances",CoverageType = "Manufacturer", PurchaseDate = new DateTime(2023, 12, 1), ExpirationDate = new DateTime(2025, 12, 1), Status = "expired",       Notes = "Daily use espresso machine." },
            new Warranty { Id = 10, UserId = AliceId, ProductId = 17, ProductName = "Sonos Arc Soundbar",     Brand = "Sonos",    Category = "Audio",             CoverageType = "Manufacturer", PurchaseDate = new DateTime(2024, 3, 15), ExpirationDate = new DateTime(2025, 3, 15), Status = "expired",       Notes = "Living room setup." },

            // Bob – Products: 1,3,6,7,8,11,13,15,16,20
            new Warranty { Id = 11, UserId = BobId,   ProductId = 1,  ProductName = "MacBook Pro 16\"",        Brand = "Apple",    Category = "Laptops",           CoverageType = "Manufacturer", PurchaseDate = new DateTime(2025, 6, 1),  ExpirationDate = new DateTime(2026, 6, 1),  Status = "active",        Notes = "Work laptop. Standard 1-year only." },
            new Warranty { Id = 12, UserId = BobId,   ProductId = 3,  ProductName = "Dyson V15 Detect",        Brand = "Dyson",    Category = "Home Appliances",   CoverageType = "Extended",     PurchaseDate = new DateTime(2024, 1, 10), ExpirationDate = new DateTime(2028, 1, 10), Status = "active",        Notes = "Extended guarantee via Dyson website." },
            new Warranty { Id = 13, UserId = BobId,   ProductId = 6,  ProductName = "KitchenAid Artisan Stand Mixer", Brand = "KitchenAid", Category = "Kitchen Appliances", CoverageType = "Manufacturer", PurchaseDate = new DateTime(2022, 9, 15), ExpirationDate = new DateTime(2027, 9, 15), Status = "active", Notes = "Wedding gift — 5-year manufacturer warranty." },
            new Warranty { Id = 14, UserId = BobId,   ProductId = 7,  ProductName = "Dell XPS 15",             Brand = "Dell",     Category = "Laptops",           CoverageType = "Extended",     PurchaseDate = new DateTime(2024, 4, 5),  ExpirationDate = new DateTime(2028, 4, 5),  Status = "active",        Notes = "Premium Support Plus active." },
            new Warranty { Id = 15, UserId = BobId,   ProductId = 8,  ProductName = "Bose QuietComfort Ultra", Brand = "Bose",     Category = "Audio",             CoverageType = "Manufacturer", PurchaseDate = new DateTime(2025, 1, 20), ExpirationDate = new DateTime(2026, 1, 20), Status = "expired",       Notes = "Bought refurbished from Bose outlet." },
            new Warranty { Id = 16, UserId = BobId,   ProductId = 11, ProductName = "Roomba j9+",              Brand = "iRobot",   Category = "Home Appliances",   CoverageType = "Extended",     PurchaseDate = new DateTime(2024, 7, 4),  ExpirationDate = new DateTime(2027, 7, 4),  Status = "active",        Notes = "iRobot Complete Care plan." },
            new Warranty { Id = 17, UserId = BobId,   ProductId = 13, ProductName = "Samsung 990 Pro 2TB SSD", Brand = "Samsung",  Category = "Storage",           CoverageType = "Manufacturer", PurchaseDate = new DateTime(2024, 2, 28), ExpirationDate = new DateTime(2029, 2, 28), Status = "active",        Notes = "Installed in desktop PC. 5-year warranty." },
            new Warranty { Id = 18, UserId = BobId,   ProductId = 15, ProductName = "LG WashTower",            Brand = "LG",       Category = "Home Appliances",   CoverageType = "Extended",     PurchaseDate = new DateTime(2023, 11, 20),ExpirationDate = new DateTime(2028, 11, 20),Status = "active",        Notes = "Premium Care plan purchased from Home Depot." },
            new Warranty { Id = 19, UserId = BobId,   ProductId = 16, ProductName = "Nintendo Switch 2",       Brand = "Nintendo",  Category = "Gaming",           CoverageType = "Manufacturer", PurchaseDate = new DateTime(2025, 6, 5),  ExpirationDate = new DateTime(2026, 6, 5),  Status = "active",        Notes = "Launch day purchase." },
            new Warranty { Id = 20, UserId = BobId,   ProductId = 20, ProductName = "Whirlpool French Door Refrigerator", Brand = "Whirlpool", Category = "Kitchen Appliances", CoverageType = "Extended", PurchaseDate = new DateTime(2023, 3, 10), ExpirationDate = new DateTime(2028, 3, 10), Status = "active", Notes = "Extended service plan. Ice maker replaced once." },

            // Carol – Products: 2,4,5,8,9,10,14,18,19,20
            new Warranty { Id = 21, UserId = CarolId, ProductId = 2,  ProductName = "Galaxy S25 Ultra",        Brand = "Samsung",  Category = "Smartphones",       CoverageType = "Extended",     PurchaseDate = new DateTime(2025, 4, 1),  ExpirationDate = new DateTime(2027, 4, 1),  Status = "active",        Notes = "Samsung Care+ with theft protection." },
            new Warranty { Id = 22, UserId = CarolId, ProductId = 4,  ProductName = "Sony WH-1000XM5",        Brand = "Sony",     Category = "Audio",             CoverageType = "Manufacturer", PurchaseDate = new DateTime(2024, 10, 15),ExpirationDate = new DateTime(2025, 10, 15),Status = "expired",       Notes = "Amazon purchase. Standard warranty." },
            new Warranty { Id = 23, UserId = CarolId, ProductId = 5,  ProductName = "LG C4 65\" OLED TV",     Brand = "LG",       Category = "TVs",               CoverageType = "Manufacturer", PurchaseDate = new DateTime(2025, 7, 4),  ExpirationDate = new DateTime(2026, 7, 4),  Status = "active",        Notes = "4th of July sale. Standard warranty only." },
            new Warranty { Id = 24, UserId = CarolId, ProductId = 8,  ProductName = "Bose QuietComfort Ultra", Brand = "Bose",     Category = "Audio",             CoverageType = "Extended",     PurchaseDate = new DateTime(2025, 2, 10), ExpirationDate = new DateTime(2027, 2, 10), Status = "active",        Notes = "Extended from Bose direct." },
            new Warranty { Id = 25, UserId = CarolId, ProductId = 9,  ProductName = "iPad Pro 13\"",           Brand = "Apple",    Category = "Tablets",           CoverageType = "Manufacturer", PurchaseDate = new DateTime(2025, 8, 20), ExpirationDate = new DateTime(2026, 8, 20), Status = "active",        Notes = "Student discount purchase." },
            new Warranty { Id = 26, UserId = CarolId, ProductId = 10, ProductName = "PlayStation 5 Pro",       Brand = "Sony",     Category = "Gaming",            CoverageType = "Extended",     PurchaseDate = new DateTime(2025, 3, 1),  ExpirationDate = new DateTime(2028, 3, 1),  Status = "active",        Notes = "GameStop protection plan." },
            new Warranty { Id = 27, UserId = CarolId, ProductId = 14, ProductName = "Breville Barista Express",Brand = "Breville",  Category = "Kitchen Appliances",CoverageType = "Extended",     PurchaseDate = new DateTime(2024, 5, 15), ExpirationDate = new DateTime(2028, 5, 15), Status = "active",        Notes = "Extended coverage from Williams Sonoma." },
            new Warranty { Id = 28, UserId = CarolId, ProductId = 18, ProductName = "HP Spectre x360 16\"",   Brand = "HP",       Category = "Laptops",           CoverageType = "Extended",     PurchaseDate = new DateTime(2024, 9, 1),  ExpirationDate = new DateTime(2027, 9, 1),  Status = "active",        Notes = "HP Care Pack 3-year on-site." },
            new Warranty { Id = 29, UserId = CarolId, ProductId = 19, ProductName = "GoPro Hero 12 Black",    Brand = "GoPro",    Category = "Cameras",           CoverageType = "Extended",     PurchaseDate = new DateTime(2024, 12, 25),ExpirationDate = new DateTime(2026, 12, 25),Status = "active",        Notes = "GoPro Subscription — annual renewal." },
            new Warranty { Id = 30, UserId = CarolId, ProductId = 20, ProductName = "Whirlpool French Door Refrigerator", Brand = "Whirlpool", Category = "Kitchen Appliances", CoverageType = "Manufacturer", PurchaseDate = new DateTime(2025, 1, 5), ExpirationDate = new DateTime(2026, 1, 5), Status = "expired", Notes = "Costco purchase. Considering extended plan." }
        );
    }
}
