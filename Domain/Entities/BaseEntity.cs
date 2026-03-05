namespace Domain.Entities;

/// <summary>
/// Базовый класс для всех сущностей.
/// Содержит стандартные поля для аудита и идентификации.
/// </summary>
public class BaseEntity
{
    /// <summary>
    /// Уникальный идентификатор сущности.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Дата и время создания сущности.
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Пользователь, создавший сущность.
    /// Может быть null, если информация отсутствует.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Дата и время последнего изменения сущности.
    /// Может быть null, если изменения не производились.
    /// </summary>
    public DateTimeOffset? LastModified { get; set; }

    /// <summary>
    /// Пользователь, внесший последние изменения.
    /// Может быть null, если изменения не производились или автор неизвестен.
    /// </summary>
    public string? LastModifiedBy { get; set; }
}