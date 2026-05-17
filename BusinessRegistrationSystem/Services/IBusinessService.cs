using System.Threading.Tasks;

namespace BusinessRegistrationSystem.Services
{
    public interface IBusinessService
    {
        Task<string> SearchNameAsync(string searchText);
    }
}
