using BCryptLib = BCrypt.Net.BCrypt;
using Application.Common.Interfaces;

namespace Infrastructure.Identity
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCryptLib.HashPassword(password, BCryptLib.GenerateSalt(12));
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            try
            {
                return BCryptLib.Verify(password, passwordHash);
            }
            catch
            {
                return false;
            }
        }
    }
}