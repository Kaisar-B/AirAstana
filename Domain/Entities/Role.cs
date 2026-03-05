namespace Domain.Entities;
public class Role : BaseEntity
{

    /// <summary>
    ///     Роль в виде кода.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    ///     Список пользователей под роль.
    /// </summary>
    public ICollection<User> Users { get; set; } = null;
}
