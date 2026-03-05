using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.FluentApiConfigurations;
/// <summary>
///     Настройка базовой доменной модели.
/// </summary>
internal class RoleEntityConfiguration : BaseEntityConfiguration, IEntityTypeConfiguration<Role>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Role> builder)
    {
        builder.Property(x => x.Code).IsRequired(true);
        builder.HasMany(x => x.Users);

        // Не кластерезованный индекс.
        builder.HasIndex(x => x.Code);

        // Size limits 
        builder.Property(r => r.Code).IsRequired().HasMaxLength(256);
    }
}
