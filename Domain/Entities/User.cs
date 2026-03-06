using Domain.Enums;

namespace Domain.Entities;
public class User : BaseEntity
{

    /// <summary>
    ///     Имя пользователя.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    ///     Хэш пароля.
    /// </summary>
    public string PasswordHashed { get; set; }

    /// <summary>
    ///     Id роли.
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    ///     Роль.
    /// </summary>
    public Role Role { get; set; }
}
