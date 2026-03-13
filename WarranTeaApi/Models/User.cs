using Microsoft.AspNetCore.Identity;

namespace WarranTeaApi.Models;

public class User : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }

    public ICollection<Warranty> Warranties { get; set; } = new List<Warranty>();
}
