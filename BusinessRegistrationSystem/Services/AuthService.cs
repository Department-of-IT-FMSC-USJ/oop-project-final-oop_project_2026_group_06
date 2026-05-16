using BusinessRegistrationSystem.Models;
using BusinessRegistrationSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BusinessRegistrationSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterAsync(string username, string password, UserRole role)
        {
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
            {
                return false;
            }

            var newUser = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                Role = role
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}