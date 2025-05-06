using Gotorz.Server.Configurations;
using Gotorz.Server.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Server.Contexts;

/// <summary>
/// Represents the database context for authentication.
/// </summary>
public class AuthDbContext : IdentityDbContext<ApplicationUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) {}

    /// <summary>
    /// Applies <see cref="ApplicationUserConfiguration"/>.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/> used to configure entity relationships.</param>
    /// <remarks>Based on suggestion from ChatGPT. Customized for this project.</remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
    }
}