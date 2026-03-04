using Domain.Enums;

namespace Domain.Entities;
public class User
{
    /// <summary>
    ///     Id.
    /// </summary>
    public int Id { get; set; }

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

    /// <summary>
    ///     Соль.
    /// </summary>
    public byte[] Salt { get; set; } = null!;
}
