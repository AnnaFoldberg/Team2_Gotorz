using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gotorz.Server.Configurations
{
    /// <summary>
    /// Configures application user entities.
    /// </summary>
    /// <remarks>Based on a ChatGPT-generated template. Customized for this project.</remarks>
    /// <author>Anna</author>
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        /// <summary>
        /// Maps ApplicationUser to the default Identity table
        /// </summary>
        /// <remarks>
        /// This is required as EF Core may otherwise map ApplicationUser to its own table in the database.
        /// </remarks>
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("AspNetUsers");
        }
    }
}