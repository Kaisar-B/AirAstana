using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.FluentApiConfigurations;
/// <summary>
///     Настройка бозовой доменной модели.
/// </summary>
internal class FlightEntityConfiguration : BaseEntityConfiguration<Flight>
{
    public void Configure(EntityTypeBuilder<Flight> builder)
    {
        builder.ToTable("Flights");

        builder.Property(x=>x.Origin).IsRequired(true);
        builder.Property(x => x.Destination).IsRequired(true);
        builder.Property(x => x.Departure).IsRequired(true);
        builder.Property(x=>x.Arrival).IsRequired(true);
        builder.Property<Enum>(x=>x.Status).HasConversion<string>().IsRequired(true);

        // Не кластерезованный индекс.
        builder.HasIndex(x => x.Origin);
        builder.HasIndex(x => x.Destination);

        // Size limits
        builder.Property(f => f.Origin).IsRequired().HasMaxLength(256);
        builder.Property(f => f.Destination).IsRequired().HasMaxLength(256);
        builder.HasBaseType(typeof(BaseEntity));
    }
}
