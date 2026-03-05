using Domain.Enums;

namespace Domain.Entities;
public class Flight : BaseEntity
{
    
    /// <summary>
    /// Место отправки.
    /// </summary>
    public string Origin { get; set; }

    /// <summary>
    /// Место прибытие.
    /// </summary>
    public string Destination { get; set; }

    /// <summary>
    /// Время вылета (время с сохранённым смещением относительно UTC).
    /// </summary>
    public DateTimeOffset Departure { get; set; }

    /// <summary>
    /// Время прилета (время с сохранённым смещением относительно UTC).
    /// </summary>
    public DateTimeOffset Arrival { get; set; }

    /// <summary>
    /// Статус рейса.
    /// </summary>
    public Status Status { get; set; }
}
