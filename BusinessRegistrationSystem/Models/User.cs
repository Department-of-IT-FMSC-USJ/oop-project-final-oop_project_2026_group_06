using System.ComponentModel.DataAnnotations;
namespace BusinessRegistrationSystem.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        [Required]
        [StringLength(50), MinLength(3)]
        public string Username { get; set; } = string.Empty;


       [Required]
       [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
       [DataType(DataType.Password)]
       [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
        ErrorMessage = "Password must contain uppercase, lowercase, number, and special character.")]
       public string Password { get; set; } = string.Empty;
       public string PasswordHash { get; set; } = string.Empty;
        


        [Required]
        public UserRole Role { get; set; }
    }
}