

namespace Domain.Entities;
public class Role
{
    /// <summary>
    ///     Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Роль в виде кода.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    ///     Список пользователей под роль.
    /// </summary>
    public ICollection<User> Users { get; set; } = null;
}
