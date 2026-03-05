using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Identity.Client;

namespace Infrastructure.Repository.FluentApiConfigurations;

/// <summary>
///     Настройка базовой доменной модели.
/// </summary>
internal class UserEntityConfiguration : BaseEntityConfiguration, IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x=>x.Username).IsRequired(true);
        builder.Property(x => x.PasswordHashed).IsRequired(true);
        builder.HasOne(x=>x.Role).WithMany(x=>x.Users)
            .HasForeignKey(x=>x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x=>x.Salt).IsRequired(true);

        // Не кластерезованный индекс.
        builder.HasIndex(x => x.Username);

        // Size limits
        builder.Property(u => u.Username).IsRequired().HasMaxLength(256);
        builder.Property(u => u.PasswordHashed).IsRequired().HasMaxLength(256);

    }
}
