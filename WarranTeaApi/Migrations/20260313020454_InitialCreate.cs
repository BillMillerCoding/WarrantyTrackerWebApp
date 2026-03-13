using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WarranTeaApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductWarranties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoverageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationMonths = table.Column<int>(type: "int", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductWarranties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductWarranties_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Warranties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoverageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiptUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarrantyDocUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warranties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warranties_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Warranties_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e5555555-5555-5555-5555-555555555555", "e5555555-5555-5555-5555-555555555555", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "AvatarUrl", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "a1111111-1111-1111-1111-111111111111", 0, null, "a1111111-1111-1111-1111-111111111111", "alice@warrantea.com", true, false, null, "Alice Johnson", "ALICE@WARRANTEA.COM", "ALICE@WARRANTEA.COM", "AQAAAAIAAYagAAAAEMgiG61xRN17mHZ+k+BC3HvTdCOoDUVQ9g5vhfRPuEFALWkOJ8210fvk1p0ZGTkiEA==", null, false, "ALICE_STAMP", false, "alice@warrantea.com" },
                    { "b2222222-2222-2222-2222-222222222222", 0, null, "b2222222-2222-2222-2222-222222222222", "bob@warrantea.com", true, false, null, "Bob Smith", "BOB@WARRANTEA.COM", "BOB@WARRANTEA.COM", "AQAAAAIAAYagAAAAEKJuR6jqLqYCbb+eg7NWNtXTRqCDk+DYas3WVRs7RE7XJLay1lwN1zFkNANUZBuU6Q==", null, false, "BOB_STAMP", false, "bob@warrantea.com" },
                    { "c3333333-3333-3333-3333-333333333333", 0, null, "c3333333-3333-3333-3333-333333333333", "carol@warrantea.com", true, false, null, "Carol Davis", "CAROL@WARRANTEA.COM", "CAROL@WARRANTEA.COM", "AQAAAAIAAYagAAAAEKpglDewuRVeSCltWMWRaxhXPyVh0D1QMzFwxiPP4NGd+XCJq4CkiW9ry9A1wsypew==", null, false, "CAROL_STAMP", false, "carol@warrantea.com" },
                    { "d4444444-4444-4444-4444-444444444444", 0, null, "d4444444-4444-4444-4444-444444444444", "admin@warrantea.com", true, false, null, "Admin User", "ADMIN@WARRANTEA.COM", "ADMIN@WARRANTEA.COM", "AQAAAAIAAYagAAAAEEzd1J0oDvs10mJQ10SQ58vj3EUUu/1onvgrhlWffWKg+E74tuNXC30tmizKDgVxHQ==", null, false, "ADMIN_STAMP", false, "admin@warrantea.com" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "Category", "Description", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, "Apple", "Laptops", "Apple's flagship professional laptop.", null, "MacBook Pro 16\"" },
                    { 2, "Samsung", "Smartphones", "Samsung's premium flagship smartphone.", null, "Galaxy S25 Ultra" },
                    { 3, "Dyson", "Home Appliances", "Cordless vacuum with laser dust detection.", null, "Dyson V15 Detect" },
                    { 4, "Sony", "Audio", "Industry-leading noise cancelling headphones.", null, "Sony WH-1000XM5" },
                    { 5, "LG", "TVs", "65-inch OLED 4K smart TV.", null, "LG C4 65\" OLED TV" },
                    { 6, "KitchenAid", "Kitchen Appliances", "Iconic tilt-head stand mixer.", null, "KitchenAid Artisan Stand Mixer" },
                    { 7, "Dell", "Laptops", "Premium 15-inch ultrabook.", null, "Dell XPS 15" },
                    { 8, "Bose", "Audio", "Premium noise cancelling headphones with spatial audio.", null, "Bose QuietComfort Ultra" },
                    { 9, "Apple", "Tablets", "Apple's most advanced tablet with M4 chip.", null, "iPad Pro 13\"" },
                    { 10, "Sony", "Gaming", "Next-gen gaming console with enhanced GPU.", null, "PlayStation 5 Pro" },
                    { 11, "iRobot", "Home Appliances", "Self-emptying robot vacuum with obstacle avoidance.", null, "Roomba j9+" },
                    { 12, "Canon", "Cameras", "Full-frame mirrorless camera with 24.2 MP sensor.", null, "Canon EOS R6 Mark II" },
                    { 13, "Samsung", "Storage", "PCIe 4.0 NVMe M.2 solid state drive.", null, "Samsung 990 Pro 2TB SSD" },
                    { 14, "Breville", "Kitchen Appliances", "Semi-automatic espresso machine with built-in grinder.", null, "Breville Barista Express" },
                    { 15, "LG", "Home Appliances", "Single-unit washer and dryer tower with AI technology.", null, "LG WashTower" },
                    { 16, "Nintendo", "Gaming", "Hybrid home and portable gaming console.", null, "Nintendo Switch 2" },
                    { 17, "Sonos", "Audio", "Premium Dolby Atmos soundbar for home theater.", null, "Sonos Arc Soundbar" },
                    { 18, "HP", "Laptops", "Premium 2-in-1 convertible laptop.", null, "HP Spectre x360 16\"" },
                    { 19, "GoPro", "Cameras", "Waterproof action camera with HyperSmooth stabilization.", null, "GoPro Hero 12 Black" },
                    { 20, "Whirlpool", "Kitchen Appliances", "36-inch counter-depth French door refrigerator.", null, "Whirlpool French Door Refrigerator" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "e5555555-5555-5555-5555-555555555555", "d4444444-4444-4444-4444-444444444444" });

            migrationBuilder.InsertData(
                table: "Warranties",
                columns: new[] { "Id", "Brand", "Category", "CoverageType", "ExpirationDate", "Notes", "ProductId", "ProductName", "PurchaseDate", "ReceiptUrl", "Status", "UserId", "WarrantyDocUrl" },
                values: new object[,]
                {
                    { 1, "Apple", "Laptops", "Extended", new DateTime(2028, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "AppleCare+ purchased at time of sale.", 1, "MacBook Pro 16\"", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "a1111111-1111-1111-1111-111111111111", null },
                    { 2, "Samsung", "Smartphones", "Manufacturer", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Carrier-purchased device.", 2, "Galaxy S25 Ultra", new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "a1111111-1111-1111-1111-111111111111", null },
                    { 3, "Dyson", "Home Appliances", "Manufacturer", new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Registered on Dyson website.", 3, "Dyson V15 Detect", new DateTime(2024, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "a1111111-1111-1111-1111-111111111111", null },
                    { 4, "Sony", "Audio", "Extended", new DateTime(2026, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Best Buy Geek Squad protection.", 4, "Sony WH-1000XM5", new DateTime(2024, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "a1111111-1111-1111-1111-111111111111", null },
                    { 5, "LG", "TVs", "Extended", new DateTime(2027, 12, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Post-holiday purchase. Wall mounted.", 5, "LG C4 65\" OLED TV", new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "a1111111-1111-1111-1111-111111111111", null },
                    { 6, "Apple", "Tablets", "Extended", new DateTime(2027, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Uses Magic Keyboard accessory.", 9, "iPad Pro 13\"", new DateTime(2025, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "a1111111-1111-1111-1111-111111111111", null },
                    { 7, "Sony", "Gaming", "Manufacturer", new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gift from spouse.", 10, "PlayStation 5 Pro", new DateTime(2025, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "expiring-soon", "a1111111-1111-1111-1111-111111111111", null },
                    { 8, "Canon", "Cameras", "Extended", new DateTime(2027, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Canon CarePAK PLUS registered.", 12, "Canon EOS R6 Mark II", new DateTime(2024, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "a1111111-1111-1111-1111-111111111111", null },
                    { 9, "Breville", "Kitchen Appliances", "Manufacturer", new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Daily use espresso machine.", 14, "Breville Barista Express", new DateTime(2023, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "expired", "a1111111-1111-1111-1111-111111111111", null },
                    { 10, "Sonos", "Audio", "Manufacturer", new DateTime(2025, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Living room setup.", 17, "Sonos Arc Soundbar", new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "expired", "a1111111-1111-1111-1111-111111111111", null },
                    { 11, "Apple", "Laptops", "Manufacturer", new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Work laptop. Standard 1-year only.", 1, "MacBook Pro 16\"", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "b2222222-2222-2222-2222-222222222222", null },
                    { 12, "Dyson", "Home Appliances", "Extended", new DateTime(2028, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Extended guarantee via Dyson website.", 3, "Dyson V15 Detect", new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "b2222222-2222-2222-2222-222222222222", null },
                    { 13, "KitchenAid", "Kitchen Appliances", "Manufacturer", new DateTime(2027, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wedding gift — 5-year manufacturer warranty.", 6, "KitchenAid Artisan Stand Mixer", new DateTime(2022, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "b2222222-2222-2222-2222-222222222222", null },
                    { 14, "Dell", "Laptops", "Extended", new DateTime(2028, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Premium Support Plus active.", 7, "Dell XPS 15", new DateTime(2024, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "b2222222-2222-2222-2222-222222222222", null },
                    { 15, "Bose", "Audio", "Manufacturer", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bought refurbished from Bose outlet.", 8, "Bose QuietComfort Ultra", new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "expired", "b2222222-2222-2222-2222-222222222222", null },
                    { 16, "iRobot", "Home Appliances", "Extended", new DateTime(2027, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "iRobot Complete Care plan.", 11, "Roomba j9+", new DateTime(2024, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "b2222222-2222-2222-2222-222222222222", null },
                    { 17, "Samsung", "Storage", "Manufacturer", new DateTime(2029, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Installed in desktop PC. 5-year warranty.", 13, "Samsung 990 Pro 2TB SSD", new DateTime(2024, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "b2222222-2222-2222-2222-222222222222", null },
                    { 18, "LG", "Home Appliances", "Extended", new DateTime(2028, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Premium Care plan purchased from Home Depot.", 15, "LG WashTower", new DateTime(2023, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "b2222222-2222-2222-2222-222222222222", null },
                    { 19, "Nintendo", "Gaming", "Manufacturer", new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Launch day purchase.", 16, "Nintendo Switch 2", new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "b2222222-2222-2222-2222-222222222222", null },
                    { 20, "Whirlpool", "Kitchen Appliances", "Extended", new DateTime(2028, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Extended service plan. Ice maker replaced once.", 20, "Whirlpool French Door Refrigerator", new DateTime(2023, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "b2222222-2222-2222-2222-222222222222", null },
                    { 21, "Samsung", "Smartphones", "Extended", new DateTime(2027, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Samsung Care+ with theft protection.", 2, "Galaxy S25 Ultra", new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "c3333333-3333-3333-3333-333333333333", null },
                    { 22, "Sony", "Audio", "Manufacturer", new DateTime(2025, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Amazon purchase. Standard warranty.", 4, "Sony WH-1000XM5", new DateTime(2024, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "expired", "c3333333-3333-3333-3333-333333333333", null },
                    { 23, "LG", "TVs", "Manufacturer", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "4th of July sale. Standard warranty only.", 5, "LG C4 65\" OLED TV", new DateTime(2025, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "c3333333-3333-3333-3333-333333333333", null },
                    { 24, "Bose", "Audio", "Extended", new DateTime(2027, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Extended from Bose direct.", 8, "Bose QuietComfort Ultra", new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "c3333333-3333-3333-3333-333333333333", null },
                    { 25, "Apple", "Tablets", "Manufacturer", new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Student discount purchase.", 9, "iPad Pro 13\"", new DateTime(2025, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "c3333333-3333-3333-3333-333333333333", null },
                    { 26, "Sony", "Gaming", "Extended", new DateTime(2028, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "GameStop protection plan.", 10, "PlayStation 5 Pro", new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "c3333333-3333-3333-3333-333333333333", null },
                    { 27, "Breville", "Kitchen Appliances", "Extended", new DateTime(2028, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Extended coverage from Williams Sonoma.", 14, "Breville Barista Express", new DateTime(2024, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "c3333333-3333-3333-3333-333333333333", null },
                    { 28, "HP", "Laptops", "Extended", new DateTime(2027, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "HP Care Pack 3-year on-site.", 18, "HP Spectre x360 16\"", new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "c3333333-3333-3333-3333-333333333333", null },
                    { 29, "GoPro", "Cameras", "Extended", new DateTime(2026, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "GoPro Subscription — annual renewal.", 19, "GoPro Hero 12 Black", new DateTime(2024, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "active", "c3333333-3333-3333-3333-333333333333", null },
                    { 30, "Whirlpool", "Kitchen Appliances", "Manufacturer", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Costco purchase. Considering extended plan.", 20, "Whirlpool French Door Refrigerator", new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "expired", "c3333333-3333-3333-3333-333333333333", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductWarranties_ProductId",
                table: "ProductWarranties",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_ProductId",
                table: "Warranties",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_UserId",
                table: "Warranties",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ProductWarranties");

            migrationBuilder.DropTable(
                name: "Warranties");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
