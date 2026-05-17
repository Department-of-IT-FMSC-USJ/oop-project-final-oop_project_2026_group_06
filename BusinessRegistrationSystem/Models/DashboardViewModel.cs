using BusinessRegistrationSystem.Models;

namespace BusinessRegistrationSystem.Models
{
    public class DashboardViewModel
    {
        public User User { get; set; } = new();
        public List<BusinessRegistration> Registrations { get; set; } = new();
    }
}
