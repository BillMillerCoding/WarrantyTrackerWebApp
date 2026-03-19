using WarranTeaApi.Models.Dto;

namespace WarranTeaApi.Data;

public static class ProductWarrantySeedData
{
    public static List<CreateProductWarrantyDto> GetAll() =>
    [
        // Product 1 – MacBook Pro 16" (5)
        new() { ProductId = 1,  CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "Apple",                    Description = "Covers manufacturing defects in materials and workmanship." },
        new() { ProductId = 1,  CoverageType = "Extended",           DurationMonths = 36,  Provider = "Apple (AppleCare+)",       Description = "Extended coverage including accidental damage protection." },
        new() { ProductId = 1,  CoverageType = "Accidental",         DurationMonths = 24,  Provider = "Apple (AppleCare+)",       Description = "Up to two incidents of accidental damage per year." },
        new() { ProductId = 1,  CoverageType = "Theft & Loss",       DurationMonths = 24,  Provider = "Apple (AppleCare+ T&L)",   Description = "Covers theft and loss with a deductible." },
        new() { ProductId = 1,  CoverageType = "International",      DurationMonths = 12,  Provider = "Apple",                    Description = "Service available at any Apple Authorized Service Provider worldwide." },

        // Product 2 – Galaxy S25 Ultra (4)
        new() { ProductId = 2,  CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "Samsung",                  Description = "Standard Samsung warranty covers hardware defects." },
        new() { ProductId = 2,  CoverageType = "Extended",           DurationMonths = 24,  Provider = "Samsung Care+",            Description = "Extended coverage with accidental damage from handling." },
        new() { ProductId = 2,  CoverageType = "Accidental",         DurationMonths = 24,  Provider = "Samsung Care+",            Description = "Cracked screen and back glass repair coverage." },
        new() { ProductId = 2,  CoverageType = "Theft & Loss",       DurationMonths = 24,  Provider = "Samsung Care+",            Description = "Device replacement for theft or loss up to $1500." },

        // Product 3 – Dyson V15 Detect (3)
        new() { ProductId = 3,  CoverageType = "Manufacturer",      DurationMonths = 24,  Provider = "Dyson",                    Description = "Parts and labor coverage for manufacturing defects." },
        new() { ProductId = 3,  CoverageType = "Extended",           DurationMonths = 48,  Provider = "Dyson",                    Description = "Dyson extended guarantee adds two extra years of parts and labor." },
        new() { ProductId = 3,  CoverageType = "Parts Only",         DurationMonths = 60,  Provider = "Dyson",                    Description = "Replacement parts coverage beyond the standard warranty." },

        // Product 4 – Sony WH-1000XM5 (3)
        new() { ProductId = 4,  CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "Sony",                     Description = "Covers defects in materials and workmanship." },
        new() { ProductId = 4,  CoverageType = "Extended",           DurationMonths = 24,  Provider = "Sony (Protection Plus)",   Description = "Extended service plan with drop and spill coverage." },
        new() { ProductId = 4,  CoverageType = "Accidental",         DurationMonths = 24,  Provider = "Sony (Protection Plus)",   Description = "Accidental damage from handling including drops." },

        // Product 5 – LG C4 65" OLED TV (4)
        new() { ProductId = 5,  CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "LG",                       Description = "In-home service for eligible repairs during warranty period." },
        new() { ProductId = 5,  CoverageType = "Extended",           DurationMonths = 36,  Provider = "LG (Premium Care)",        Description = "Extended in-home service and accidental damage coverage." },
        new() { ProductId = 5,  CoverageType = "Accidental",         DurationMonths = 36,  Provider = "LG (Premium Care)",        Description = "Covers accidental damage including cracked screens." },
        new() { ProductId = 5,  CoverageType = "Panel Replacement",  DurationMonths = 60,  Provider = "LG",                       Description = "OLED panel burn-in replacement guarantee." },

        // Product 6 – KitchenAid Artisan Stand Mixer (3)
        new() { ProductId = 6,  CoverageType = "Manufacturer",      DurationMonths = 60,  Provider = "KitchenAid",               Description = "Hassle-free replacement warranty for manufacturing defects." },
        new() { ProductId = 6,  CoverageType = "Extended",           DurationMonths = 84,  Provider = "KitchenAid",               Description = "Extended 7-year coverage for motor and gearbox." },
        new() { ProductId = 6,  CoverageType = "Commercial",         DurationMonths = 24,  Provider = "KitchenAid Commercial",    Description = "Commercial-use warranty with expedited replacement." },

        // Product 7 – Dell XPS 15 (4)
        new() { ProductId = 7,  CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "Dell",                     Description = "Basic hardware warranty with mail-in service." },
        new() { ProductId = 7,  CoverageType = "Extended",           DurationMonths = 48,  Provider = "Dell (Premium Support Plus)", Description = "24/7 tech support and next-business-day on-site service." },
        new() { ProductId = 7,  CoverageType = "Accidental",         DurationMonths = 48,  Provider = "Dell (Accidental Damage)", Description = "Covers drops, spills, surges, and screen cracks." },
        new() { ProductId = 7,  CoverageType = "International",      DurationMonths = 48,  Provider = "Dell (International Travel)", Description = "Service coverage when traveling outside your home country." },

        // Product 8 – Bose QuietComfort Ultra (3)
        new() { ProductId = 8,  CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "Bose",                     Description = "Coverage against defects in materials and workmanship." },
        new() { ProductId = 8,  CoverageType = "Extended",           DurationMonths = 24,  Provider = "Bose",                     Description = "Extended replacement warranty with trade-in option." },
        new() { ProductId = 8,  CoverageType = "Accidental",         DurationMonths = 24,  Provider = "Bose (Product Protection)", Description = "Covers accidental damage from drops and liquid spills." },

        // Product 9 – iPad Pro 13" (4)
        new() { ProductId = 9,  CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "Apple",                    Description = "One-year limited warranty for manufacturing defects." },
        new() { ProductId = 9,  CoverageType = "Extended",           DurationMonths = 24,  Provider = "Apple (AppleCare+)",       Description = "Extended coverage with Express Replacement Service." },
        new() { ProductId = 9,  CoverageType = "Accidental",         DurationMonths = 24,  Provider = "Apple (AppleCare+)",       Description = "Two incidents of accidental damage per year." },
        new() { ProductId = 9,  CoverageType = "Theft & Loss",       DurationMonths = 24,  Provider = "Apple (AppleCare+ T&L)",   Description = "Theft and loss coverage with deductible." },

        // Product 10 – PlayStation 5 Pro (3)
        new() { ProductId = 10, CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "Sony Interactive",          Description = "Standard manufacturer warranty for hardware defects." },
        new() { ProductId = 10, CoverageType = "Extended",           DurationMonths = 36,  Provider = "PlayStation (Protection Plan)", Description = "Extended protection including controller coverage." },
        new() { ProductId = 10, CoverageType = "Accidental",         DurationMonths = 24,  Provider = "PlayStation (Protection Plan)", Description = "Covers power surges, drops, and dust damage." },

        // Product 11 – Roomba j9+ (4)
        new() { ProductId = 11, CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "iRobot",                   Description = "Standard warranty covering hardware and software defects." },
        new() { ProductId = 11, CoverageType = "Extended",           DurationMonths = 36,  Provider = "iRobot (Complete Care)",    Description = "Extended coverage with battery replacement included." },
        new() { ProductId = 11, CoverageType = "Parts Only",         DurationMonths = 24,  Provider = "iRobot",                   Description = "Replacement brushes, filters, and bags for the life of the plan." },
        new() { ProductId = 11, CoverageType = "Accidental",         DurationMonths = 24,  Provider = "iRobot (Complete Care)",    Description = "Covers liquid damage from mopping malfunctions." },

        // Product 12 – Canon EOS R6 Mark II (4)
        new() { ProductId = 12, CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "Canon",                    Description = "One-year warranty against manufacturing defects." },
        new() { ProductId = 12, CoverageType = "Extended",           DurationMonths = 36,  Provider = "Canon (CarePAK PLUS)",     Description = "Extends original warranty with accidental damage protection." },
        new() { ProductId = 12, CoverageType = "Accidental",         DurationMonths = 24,  Provider = "Canon (CarePAK PLUS)",     Description = "Covers drops, spills, and power surges." },
        new() { ProductId = 12, CoverageType = "Professional",       DurationMonths = 12,  Provider = "Canon (CPS Platinum)",     Description = "Priority repair and loaner equipment for professional photographers." },

        // Product 13 – Samsung 990 Pro 2TB SSD (3)
        new() { ProductId = 13, CoverageType = "Manufacturer",      DurationMonths = 60,  Provider = "Samsung",                  Description = "5-year limited warranty or 2,400 TBW, whichever comes first." },
        new() { ProductId = 13, CoverageType = "Extended",           DurationMonths = 120, Provider = "Samsung",                  Description = "10-year extended warranty with data migration assistance." },
        new() { ProductId = 13, CoverageType = "Data Recovery",      DurationMonths = 36,  Provider = "Samsung (Data Guard)",     Description = "Professional data recovery service coverage." },

        // Product 14 – Breville Barista Express (4)
        new() { ProductId = 14, CoverageType = "Manufacturer",      DurationMonths = 24,  Provider = "Breville",                 Description = "Two-year repair or replace warranty for defects." },
        new() { ProductId = 14, CoverageType = "Extended",           DurationMonths = 48,  Provider = "Breville",                 Description = "Extended warranty covering internal components and boiler." },
        new() { ProductId = 14, CoverageType = "Replacement",        DurationMonths = 12,  Provider = "Breville",                 Description = "Head unit replacement within first year if defective." },
        new() { ProductId = 14, CoverageType = "Commercial",         DurationMonths = 12,  Provider = "Breville Commercial",      Description = "Limited commercial-use warranty for small businesses." },

        // Product 15 – LG WashTower (4)
        new() { ProductId = 15, CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "LG",                       Description = "Parts and labor warranty for manufacturing defects." },
        new() { ProductId = 15, CoverageType = "Extended",           DurationMonths = 60,  Provider = "LG (Premium Care)",        Description = "Five-year extended parts and labor coverage." },
        new() { ProductId = 15, CoverageType = "Parts Only",         DurationMonths = 120, Provider = "LG",                       Description = "10-year motor and compressor limited warranty." },
        new() { ProductId = 15, CoverageType = "In-Home Service",    DurationMonths = 36,  Provider = "LG (Premium Care)",        Description = "In-home technician service for major repairs." },

        // Product 16 – Nintendo Switch 2 (3)
        new() { ProductId = 16, CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "Nintendo",                 Description = "Standard warranty covering hardware defects." },
        new() { ProductId = 16, CoverageType = "Extended",           DurationMonths = 36,  Provider = "Nintendo",                 Description = "Extended protection plan with Joy-Con drift coverage." },
        new() { ProductId = 16, CoverageType = "Accidental",         DurationMonths = 24,  Provider = "Nintendo",                 Description = "Covers accidental drops and screen cracks." },

        // Product 17 – Sonos Arc Soundbar (3)
        new() { ProductId = 17, CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "Sonos",                    Description = "One-year limited warranty for hardware defects." },
        new() { ProductId = 17, CoverageType = "Extended",           DurationMonths = 36,  Provider = "Sonos",                    Description = "Extended warranty with advance replacement option." },
        new() { ProductId = 17, CoverageType = "Replacement",        DurationMonths = 24,  Provider = "Sonos",                    Description = "Replacement unit provided within 2 business days." },

        // Product 18 – HP Spectre x360 16" (4)
        new() { ProductId = 18, CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "HP",                       Description = "Standard limited warranty with mail-in service." },
        new() { ProductId = 18, CoverageType = "Extended",           DurationMonths = 36,  Provider = "HP (Care Pack)",           Description = "Three-year on-site next-business-day support." },
        new() { ProductId = 18, CoverageType = "Accidental",         DurationMonths = 36,  Provider = "HP (Accidental Damage)",   Description = "Covers drops, spills, and electrical surges." },
        new() { ProductId = 18, CoverageType = "International",      DurationMonths = 36,  Provider = "HP (International Travel)", Description = "Worldwide service coverage for frequent travelers." },

        // Product 19 – GoPro Hero 12 Black (3)
        new() { ProductId = 19, CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "GoPro",                    Description = "One-year warranty against manufacturing defects." },
        new() { ProductId = 19, CoverageType = "Extended",           DurationMonths = 24,  Provider = "GoPro (Subscription)",     Description = "GoPro subscriber replacement — no questions asked." },
        new() { ProductId = 19, CoverageType = "Accidental",         DurationMonths = 24,  Provider = "GoPro (Subscription)",     Description = "Camera replacement for any damage, any reason." },

        // Product 20 – Whirlpool French Door Refrigerator (4)
        new() { ProductId = 20, CoverageType = "Manufacturer",      DurationMonths = 12,  Provider = "Whirlpool",                Description = "Full parts and labor warranty for one year." },
        new() { ProductId = 20, CoverageType = "Extended",           DurationMonths = 60,  Provider = "Whirlpool (Total Coverage)", Description = "Five-year extended service plan." },
        new() { ProductId = 20, CoverageType = "Parts Only",         DurationMonths = 120, Provider = "Whirlpool",                Description = "10-year limited parts warranty on cavity liner and compressor." },
        new() { ProductId = 20, CoverageType = "In-Home Service",    DurationMonths = 36,  Provider = "Whirlpool (Total Coverage)", Description = "In-home diagnosis and repair by certified technicians." },
    ];
}
