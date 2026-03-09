using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.FluentApiConfigurations;

/// <summary>
/// Конфигурация Fluent API для базовой сущности BaseEntity.
/// Настраивает ключи и обязательные/необязательные поля.
/// </summary>
internal class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        // Устанавливаем первичный ключ
        builder.HasKey(x => x.Id);

        // Обязательные поля
        builder.Property(x => x.Created).IsRequired(true);
        builder.Property(x => x.CreatedBy).IsRequired(true);

        // Необязательные поля
        builder.Property(x => x.LastModified).IsRequired(false);
        builder.Property(x => x.LastModifiedBy).IsRequired(false);
    }
}