using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BusinessRegistrationSystem.Models
{
    public class BusinessRegistration
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(200)]
        public string ReservationName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? SinhalaName { get; set; }

        [StringLength(200)]
        public string? TamilName { get; set; }

        [StringLength(500)]
        public string? Abbreviations { get; set; }

        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        public string District { get; set; } = string.Empty;

        [StringLength(100)]
        public string DivisionalSecretariatDivision { get; set; } = string.Empty;

        [StringLength(100)]
        public string GNDivision { get; set; } = string.Empty;

        [StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [StringLength(100)]
        public string CompanyEmail { get; set; } = string.Empty;

        [StringLength(20)]
        public string CompanyPhoneNumber { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Objectives { get; set; } = string.Empty;

        [StringLength(2000)]
        public string NatureOfBusiness { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Pending";

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid OwnerId { get; set; }
        
        public User? Owner { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalCapital { get; set; }

        public List<Director> Directors { get; set; } = new();
        public List<Shareholder> Shareholders { get; set; } = new();
        
        public Secretary? Secretary { get; set; }
    }
}
