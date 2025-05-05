using Gotorz.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gotorz.Server.Configurations
{
    public class HolidayPackageConfiguration : IEntityTypeConfiguration<HolidayPackage>
    {
        public void Configure(EntityTypeBuilder<HolidayPackage> builder)
        {
            builder.Property(p => p.CostPrice).HasColumnType("decimal(18,2)");
            builder.Property(p => p.MarkupPercentage).HasColumnType("decimal(5,2)");
        }
    }
}