using BusinessRegistrationSystem.Models;

namespace BusinessRegistrationSystem.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string username, string password, UserRole role);
        Task<User?> LoginAsync(string username, string password);
    }
}